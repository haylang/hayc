using System.Text;
using HaycLib.Ast;
using HaycLib.Ast.Data;
using HaycLib.Ast.Nodes;
using HaycLib.Extensions;
using HaycLib.Lexing;
using HaycLib.Location;
using HaycLib.Reporting;
using Range = HaycLib.Location.Range;

namespace HaycLib.Parsing;

public sealed class Parser
{
    public Parser(List<Token> tokens)
    {
        _state = new ParserState(tokens);
    }

    /// <summary>
    /// The parser state.
    /// </summary>
    private readonly ParserState _state;

    /// <summary>
    /// Retrieves the messages from parsing.
    /// </summary>
    public IEnumerable<Message> GetMessages()
    {
        return _state.GetAllMessages();
    }

    public FileNode? Parse()
    {
        try
        {
            return ParseCompilationUnit();
        }
        catch (EndOfFileException)
        {
            // We didn't get past the namespace declaration.
            return null;
        }
    }

    private FileNode ParseCompilationUnit()
    {
        List<NamespaceName> imports = new();
        while (!_state.ReachedEndOfFile
               && IsImport())
        {
            imports.Add(ParseImport());
        }

        NamespaceName namespaceName;
        if (!_state.ReachedEndOfFile
            && IsNamespaceStatement())
        {
            namespaceName = ParseNamespaceStatement();
        }
        else
        {
            namespaceName = new NamespaceName("???", _state.CurrentToken.Location);
            _state.ErrCurrentToken("Expected namespace declaration.");
        }

        BlockNode body = new(namespaceName.Location);

        while (!_state.ReachedEndOfFile)
        {
            if (IsStruct())
            {
                body.Children.Add(ParseStruct());
            }
            else if (IsFuncDeclaration())
            {
                body.Children.Add(ParseFuncDeclaration());
            }
            else
            {
                _state.ErrCurrentToken("Expected struct or function.");
                break;
            }
        }

        return new FileNode(namespaceName, imports, body);
    }

    private bool IsImport()
    {
        return _state.CurrentTokenIs(TokenType.Import);
    }

    private NamespaceName ParseImport()
    {
        _state.ExpectTokenOrError(TokenType.Import);
        _state.NextToken();

        NamespaceName name = ParseNamespaceName();

        _state.ExpectTokenOrError(TokenType.Semicolon);
        _state.NextToken();

        return name;
    }

    private bool IsNamespaceStatement()
    {
        return _state.CurrentTokenIs(TokenType.Namespace);
    }

    private NamespaceName ParseNamespaceStatement()
    {
        _state.ExpectTokenOrError(TokenType.Namespace);
        _state.NextToken();

        NamespaceName name = ParseNamespaceName();

        _state.ExpectTokenOrError(TokenType.Semicolon);
        _state.NextToken();

        return name;
    }

    private bool IsStruct()
    {
        _state.BeginNewSnapshot();

        try
        {
            _state.SkipAttributes();
            _state.SkipOfType(TokenType.Modifier);

            bool isStruct = _state.CurrentTokenIs(TokenType.Struct);

            _state.RevertToPreviousSnapshot();

            return isStruct;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private StructNode ParseStruct()
    {
        IEnumerable<HayAttribute> attributes = CollectAttributes();
        IEnumerable<Token> modifiers = _state.CollectModifiers();
        _state.ErrorUnwantedModifiers(modifiers);

        _state.ExpectTokenOrError(TokenType.Struct);
        _state.NextToken();

        Token name = _state.CurrentToken;
        _state.ExpectTokenOrError(TokenType.Identifier);
        _state.NextToken();

        StructBody body = ParseStructBody();

        return new StructNode(name.Value.ToString(), body, attributes, name.Location);
    }

    private bool IsFuncDeclaration()
    {
        _state.BeginNewSnapshot();

        try
        {
            _state.SkipAttributes();
            _state.SkipOfType(TokenType.Modifier);

            bool isStruct = _state.CurrentTokenIs(TokenType.Function);

            _state.RevertToPreviousSnapshot();

            return isStruct;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private FuncNode ParseFuncDeclaration()
    {
        IEnumerable<HayAttribute> attributes = CollectAttributes();
        IEnumerable<Token> modifiers = _state.CollectModifiers();

        _state.ExpectTokenOrError(TokenType.Function);
        _state.NextToken();

        ObjAccessNode? owner = null;

        if (_state.CurrentTokenIs(TokenType.At))
        {
            _state.NextToken();
            owner = ParseObjectAccess(true);
        }

        Token nameToken = _state.CurrentToken;
        _state.ExpectTokenOrError(TokenType.Identifier);
        _state.NextToken();

        List<ObjAccessNode> genericParams = _state.CurrentTokenIs(TokenType.LeftAngle)
                                                ? ParseGenericParams()
                                                : new List<ObjAccessNode>(0);

        List<VariableNode> parameters = ParseParameters();

        ObjAccessNode returnType;
        _state.ExpectTokenOrError(TokenType.Colon);
        if (_state.CurrentTokenIs(TokenType.Colon))
        {
            _state.NextToken();
            returnType = ParseObjectAccess(true);
        }
        else
        {
            _state.ErrCurrentToken("Missing return type.");
            returnType = new ObjAccessNode(null, "<none>", Array.Empty<ObjAccessNode>(), nameToken.Location);
        }

        BlockNode body = ParseStatementBlock();

        return owner == null
                   ? new FuncNode(
                       nameToken.Value.ToString(),
                       body,
                       attributes,
                       parameters,
                       returnType,
                       nameToken.Location
                   )
                   : new StructFuncNode(
                       nameToken.Value.ToString(),
                       body,
                       attributes,
                       parameters,
                       returnType,
                       nameToken.Location,
                       owner
                   );
    }

    private StructBody ParseStructBody()
    {
        List<StructInitializerNode> initializers = new(0);
        List<VariableNode> fields = new(0);
        StructDestructorNode? destructor = null;

        if (!_state.CurrentTokenIs(TokenType.LeftBrace))
        {
            _state.ErrCurrentToken("Structure must have body (you may have missed the opening brace).");
            return new StructBody(initializers, fields, destructor);
        }

        _state.NextToken(); // skip opening brace

        while (!_state.CurrentTokenIs(TokenType.RightBrace))
        {
            if (IsStructInitializer())
            {
                initializers.Add(ParseStructInitializer());
            }
            else if (IsStructDestructor())
            {
                if (destructor != null)
                {
                    _state.ErrCurrentToken("Only one destructor is allowed.");
                }
                destructor = ParseStructDestructor();
            }
            else if (IsStructField())
            {
                fields.Add(ParseStructField());
            }
            else
            {
                _state.ErrCurrentToken(
                    $"Expected struct field, initializer or destructor (got '{_state.CurrentToken.Value}')."
                );
                _state.Recover(ParseRecoverMode.UntilBrace);
            }
        }

        _state.NextToken(); // skip }; the while loop won't exit until we're at it

        return new StructBody(initializers, fields, destructor);
    }

    private bool IsStructInitializer()
    {
        return _state.CurrentTokenIs(TokenType.Initializer);
    }

    private StructInitializerNode ParseStructInitializer()
    {
        FileLocation initializerKeywordLocation = _state.CurrentToken.Location;
        _state.ExpectTokenOrError(TokenType.Initializer);
        _state.NextToken();

        List<VariableNode> parameters = ParseParameters();
        BlockNode body = ParseStatementBlock();

        return new StructInitializerNode(body, parameters, initializerKeywordLocation);
    }

    private bool IsStructDestructor()
    {
        return _state.CurrentTokenIs(TokenType.Destructor);
    }

    private StructDestructorNode ParseStructDestructor()
    {
        FileLocation destructorKeywordLocation = _state.CurrentToken.Location;
        _state.ExpectTokenOrError(TokenType.Destructor);
        _state.NextToken();

        _state.ExpectTokenOrError(TokenType.LeftParen);
        if (_state.CurrentTokenIs(TokenType.LeftParen))
        {
            _state.NextToken();
        }

        _state.ExpectTokenOrError(TokenType.RightParen);
        if (_state.CurrentTokenIs(TokenType.RightParen))
        {
            _state.NextToken();
        }

        BlockNode body = ParseStatementBlock();

        return new StructDestructorNode(body, destructorKeywordLocation);
    }

    private bool IsStructField()
    {
        _state.BeginNewSnapshot();

        try
        {
            bool isField = IsTypeRef();
            ParseTypeRef();
            isField = isField && _state.CurrentTokenIs(TokenType.Identifier);
            _state.NextToken();

            // this cluster-fuck is:
            // - we didn't find a semicolon and the next token is = or < or (
            // - the = is for assignments
            // - the < is for generic func calls
            // - the ( is for non-generic func calls
            if (!_state.CurrentTokenIs(TokenType.Semicolon)
                && (_state.CurrentTokenIs(TokenType.Assign)
                    || _state.CurrentTokenIs(TokenType.LeftAngle)
                    || _state.CurrentTokenIs(TokenType.LeftParen)))
            {
                isField = false;
            }

            _state.RevertToPreviousSnapshot();
            return isField;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private VariableNode ParseStructField()
    {
        ObjAccessNode type = ParseTypeRef();
        Token name = _state.CurrentToken;
        _state.ExpectTokenOrError(TokenType.Identifier);
        _state.NextToken();

        if (_state.CurrentTokenIs(TokenType.Semicolon))
        {
            _state.NextToken();
        }
        else
        {
            _state.ErrCurrentToken("Missing semicolon.");
        }

        return new VariableNode(name.Value.ToString(), type, name.Location, null);
    }

    private BlockNode ParseStatementBlock()
    {
        Token openingBrace = _state.CurrentToken;
        _state.ExpectTokenOrError(TokenType.LeftBrace);
        _state.NextToken();

        BlockNode node = new(openingBrace.Location);

        while (!_state.CurrentTokenIs(TokenType.RightBrace))
        {
            AstNode? stat = ParseStatement();

            if (stat != null)
            {
                node.Children.Add(stat);
            }
        }

        _state.NextToken(); // consume }

        return node;
    }

    private AstNode? ParseStatement()
    {
        if (IsFuncCall())
        {
            return ParseFuncCall();
        }
        else if (IsLocalVariableDecl())
        {
            return ParseLocalVariableDecl();
        }
        else if (IsVariableAssignment())
        {
            return ParseVariableAssignment();
        }
        else
        {
            _state.ErrCurrentToken($"Expected statement (got '{_state.CurrentToken.Value}').");
            _state.Recover(ParseRecoverMode.Detect);
            return null;
        }
    }

    private bool IsFuncCall()
    {
        // We have two options:
        // - The function is accessed by instance
        // - The function is accessed statically (by namespace)

        _state.BeginNewSnapshot();

        try
        {
            bool isCall = false;
            if (IsObjectAccess())
            {
                // function call on struct instance
                isCall = true;
                ParseObjectAccess(true);
            }
            else if (IsTypeRef())
            {
                // same syntax as type ref; this is just a function within a namespace
                isCall = true;
                ParseTypeRef();
            }
            isCall = isCall && _state.CurrentTokenIs(TokenType.LeftParen);

            _state.RevertToPreviousSnapshot();
            return isCall;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private FuncCallNode ParseFuncCall()
    {
        ObjAccessNode functionObject;

        if (IsObjectAccess())
        {
            functionObject = ParseObjectAccess(true);
        }
        else if (IsTypeRef())
        {
            functionObject = ParseTypeRef();
        }
        else
        {
            functionObject = new ObjAccessNode(
                null,
                "???",
                Array.Empty<ObjAccessNode>(),
                _state.CurrentToken.Location
            );
            _state.ErrCurrentToken($"Expected function name (got '{_state.CurrentToken.Value}').");
        }

        List<AstNode> arguments = ParseArguments();

        return new FuncCallNode(functionObject, arguments, functionObject.SourceLocation);
    }

    private bool IsVariableAssignment()
    {
        _state.BeginNewSnapshot();

        try
        {
            bool isCall = IsObjectAccess();
            ParseObjectAccess(true);
            isCall = isCall && _state.CurrentTokenIs(TokenType.Assign);
            // TODO: More assignment operators

            _state.RevertToPreviousSnapshot();
            return isCall;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private VariableAssignmentNode ParseVariableAssignment()
    {
        ObjAccessNode variable = ParseObjectAccess(true);
        _state.ExpectTokenOrError(TokenType.Assign);
        _state.NextToken();

        AstNode value = ParseExpression();
        
        if (_state.CurrentTokenIs(TokenType.Semicolon))
        {
            _state.NextToken();
        }
        else
        {
            _state.ErrCurrentToken("Missing semicolon.");
        }

        return new VariableAssignmentNode(variable, value, variable.SourceLocation);
    }

    private bool IsLocalVariableDecl()
    {
        _state.BeginNewSnapshot();

        try
        {
            bool isDecl = IsTypeRef();
            ParseTypeRef();
            isDecl = isDecl && _state.CurrentTokenIs(TokenType.Identifier);
            _state.NextToken();
            isDecl = isDecl
                     && (_state.CurrentTokenIs(TokenType.Assign)
                         || _state.CurrentTokenIs(TokenType.Semicolon));
            _state.NextToken();

            _state.RevertToPreviousSnapshot();
            return isDecl;
        }
        catch (EndOfFileException)
        {
            _state.RevertToPreviousSnapshot();
            return false;
        }
    }

    private VariableNode ParseLocalVariableDecl()
    {
        ObjAccessNode type = ParseTypeRef();

        Token name = _state.CurrentToken;
        _state.ExpectTokenOrError(TokenType.Identifier);
        _state.NextToken();

        if (_state.CurrentTokenIs(TokenType.Assign))
        {
            _state.NextToken();
            AstNode value = ParseExpression();

            if (_state.CurrentTokenIs(TokenType.Semicolon))
            {
                _state.NextToken();
            }
            else
            {
                _state.ErrCurrentToken("Missing semicolon.");
            }

            return new VariableNode(name.Value.ToString(), type, name.Location, value);
        }
        else
        {
            if (_state.CurrentTokenIs(TokenType.Semicolon))
            {
                _state.NextToken();
            }
            else
            {
                _state.ErrCurrentToken("Missing semicolon.");
            }

            return new VariableNode(name.Value.ToString(), type, name.Location, null);
        }
    }

    private bool IsObjectAccess()
    {
        return _state.CurrentTokenIs(TokenType.Identifier)
               && _state.PeekToken().Type != TokenType.DoubleColon;
    }

    private ObjAccessNode ParseObjectAccess(bool allowGenericArguments)
    {
        // segment - word separated by .
        List<Token> segments = new(1);
        List<ObjAccessNode> genericArgs = new(0);

        while (true)
        {
            _state.ExpectTokenOrError(TokenType.Identifier);
            segments.Add(_state.CurrentToken);
            _state.NextToken();

            if (_state.CurrentTokenIs(TokenType.Dot))
            {
                _state.NextToken();
            }
            else
            {
                break;
            }
        }

        if (allowGenericArguments)
        {
            if (_state.CurrentTokenIs(TokenType.LeftAngle))
            {
                _state.NextToken();

                while (!_state.CurrentTokenIs(TokenType.RightAngle))
                {
                    genericArgs.Add(ParseTypeRef());
                }

                _state.NextToken(); // skip >
            }
        }
        else if (_state.CurrentTokenIs(TokenType.LeftAngle))
        {
            // error!

            FileLocation fullLocation = _state.SkipGenericType();

            _state.CurrentSnapshot.MessageBatch.AddError(
                "Generic arguments are not allowed at this point.",
                fullLocation
            );

            _state.NextToken(); // skip last >
        }

        NamespaceName? namespaceName = null;

        if (segments.Count > 1)
        {
            // the segments of *only* the namespace name
            IEnumerable<Token> namespaceNameSegments = segments.TakeLast(1);
            Position start = segments.First().Location.Range.Start;
            Position end = segments.Last().Location.Range.End;
            Range range = new(start, end);
            FileLocation location = _state.CurrentToken.Location with
            {
                Range = range
            };
            namespaceName = new NamespaceName(String.Join("", namespaceNameSegments), location);
        }

        Token typeNameToken = segments.Last();
        return new ObjAccessNode(
            namespaceName,
            typeNameToken.Value.ToString(),
            genericArgs,
            typeNameToken.Location
        );
    }

    private AstNode ParseExpression()
    {
        AstNode left = ParseExpression_Primary();
        return ParseExpression_Rhs(0, left);
    }

    private AstNode ParseExpression_Rhs(int minPrecedence, AstNode lhs)
    {
        Token lookahead = _state.CurrentToken;

        while (lookahead.Type.GetPrecedence() >= minPrecedence)
        {
            Token op = lookahead;
            _state.NextToken();

            AstNode rhs = ParseExpression_Primary();
            lookahead = _state.CurrentToken;

            int opPrecedence = op.Type.GetPrecedence();
            int lookaheadPrecedence = lookahead.Type.GetPrecedence();
            while (lookaheadPrecedence > opPrecedence
                   || lookahead.Type.IsRightAssociative()
                   && lookaheadPrecedence == minPrecedence)
            {
                rhs = ParseExpression_Rhs(opPrecedence + (lookaheadPrecedence > opPrecedence ? 1 : 0), rhs);
                lookahead = _state.CurrentToken;
            }

            TokenType opType = op.Type;
            lhs = opType switch
            {
                TokenType.Plus
                 or TokenType.Minus
                 or TokenType.Asterisk
                 or TokenType.Slash
                 or TokenType.Percent => new ArithmeticOperatorNode(
                        (ArithmeticOperatorType) opType,
                        lhs,
                        rhs,
                        op.Location
                    ),
                _ => throw new NotSupportedException($"Operator {opType} is not supported.")
            };
        }

        return lhs;
    }

    private AstNode ParseExpression_Primary()
    {
        if (IsObjectAccess())
        {
            return ParseObjectAccess(false);
        }
        else if (IsNumberExpr())
        {
            return ParseNumberExpr();
        }
        else if (IsParenExpr())
        {
            return ParseParenExpr();
        }
        else
        {
            FileLocation errLoc = _state.CurrentToken.Location;
            _state.ErrCurrentToken($"Expected expression (got '{_state.CurrentToken.Type}').");
            _state.Recover(ParseRecoverMode.Detect);
            return new NullNode(errLoc);
        }
    }

    private bool IsNumberExpr()
    {
        return _state.CurrentTokenIs(TokenType.Integer)
               || _state.CurrentTokenIs(TokenType.Decimal);
    }

    private AstNode ParseNumberExpr()
    {
        Token numberToken = _state.CurrentToken;
        _state.NextToken();

        switch (numberToken.Type)
        {
            case TokenType.Integer:
                return new IntegerNode(Convert.ToInt64(numberToken.Value.ToString()), numberToken.Location);

            case TokenType.Decimal:
                return new DecimalNode(Convert.ToDouble(numberToken.Value.ToString()), numberToken.Location);

            default:
                _state.Error("Expected number.", numberToken);
                return new IntegerNode(0, numberToken.Location);
        }
    }

    private bool IsParenExpr()
    {
        return _state.CurrentTokenIs(TokenType.LeftParen);
    }

    private AstNode ParseParenExpr()
    {
        _state.ExpectTokenOrError(TokenType.LeftParen);
        _state.NextToken();

        AstNode expr = ParseExpression();

        _state.ExpectTokenOrError(TokenType.RightParen);
        if (_state.CurrentTokenIs(TokenType.RightParen))
        {
            _state.NextToken();
        }

        return expr;
    }

    private List<AstNode> ParseArguments()
    {
        List<AstNode> arguments = new(0);
        if (_state.CurrentTokenIs(TokenType.LeftParen))
        {
            _state.NextToken();
        }

        while (!_state.CurrentTokenIs(TokenType.RightParen))
        {
            arguments.Add(ParseExpression());

            if (!_state.CurrentTokenIs(TokenType.RightParen)
                && !_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.ErrCurrentToken("Expected comma (to separate args).");
            }
            else if (_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.NextToken();
            }
        }
        _state.NextToken();
        return arguments;
    }

    private NamespaceName ParseNamespaceName()
    {
        _state.ExpectTokenOrError(TokenType.Identifier);

        string fileName = _state.CurrentToken.Location.Path;
        Position start = _state.CurrentToken.Location.Range.Start;
        Position end;
        StringBuilder namespaceNameBuilder = new();

        while (true)
        {
            namespaceNameBuilder.Append(_state.CurrentToken.Value);
            end = _state.CurrentToken.Location.Range.End;
            _state.NextToken();

            if (_state.CurrentTokenIs(TokenType.DoubleColon))
            {
                _state.NextToken();
            }
            else
            {
                break;
            }
        }

        Range range = new(start, end);
        FileLocation location = new(fileName, range);

        return new NamespaceName(namespaceNameBuilder.ToString(), location);
    }

    private IEnumerable<HayAttribute> CollectAttributes()
    {
        List<HayAttribute> attributes = new(0);
        while (IsAttribute())
        {
            _state.ExpectTokenOrError(TokenType.LeftBracket);
            _state.NextToken();

            Token attrName = _state.CurrentToken;
            _state.NextToken();

            _state.ExpectTokenOrError(TokenType.RightBracket);
            _state.NextToken();

            attributes.Add(new HayAttribute(attrName.Value.ToString(), attrName.Location));
        }

        return attributes;
    }

    private bool IsAttribute()
    {
        if (!_state.CurrentTokenIs(TokenType.LeftBracket))
        {
            return false;
        }

        _state.BeginNewSnapshot();
        _state.NextToken();

        bool isAttr = _state.CurrentTokenIs(TokenType.Identifier);

        _state.RevertToPreviousSnapshot();

        return isAttr;
    }

    private List<VariableNode> ParseParameters()
    {
        _state.ExpectTokenOrError(TokenType.LeftParen);
        _state.NextToken();

        List<VariableNode> parameters = new();

        while (!_state.CurrentTokenIs(TokenType.RightParen))
        {
            FileLocation location;
            ObjAccessNode type = ParseTypeRef();
            string name;

            if (_state.CurrentTokenIs(TokenType.Identifier))
            {
                name     = _state.CurrentToken.Value.ToString();
                location = _state.CurrentToken.Location;
                _state.NextToken();
            }
            else
            {
                name     = "_";
                location = type.SourceLocation;
            }

            parameters.Add(new VariableNode(name, type, location, null));

            if (!_state.CurrentTokenIs(TokenType.RightParen)
                && !_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.ErrCurrentToken("Expected comma (to separate params).");
            }
            else if (_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.NextToken();
            }
        }

        _state.NextToken(); // skip )

        return parameters;
    }

    private List<ObjAccessNode> ParseGenericParams()
    {
        if (_state.CurrentTokenIs(TokenType.LeftAngle))
        {
            _state.NextToken();
        }
        else
        {
            _state.ErrCurrentToken("Expected < for generic params.");
            return new List<ObjAccessNode>(0);
        }

        List<ObjAccessNode> arguments = new(1);
        while (!_state.CurrentTokenIs(TokenType.RightAngle))
        {
            arguments.Add(ParseTypeRef());

            if (!_state.CurrentTokenIs(TokenType.RightAngle)
                && !_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.ErrCurrentToken("Expected comma.");
            }
            else if (_state.CurrentTokenIs(TokenType.Comma))
            {
                _state.NextToken();
            }
        }
        _state.NextToken();

        return arguments;
    }

    private bool IsTypeRef()
    {
        return _state.CurrentTokenIs(TokenType.Identifier);
    }

    private ObjAccessNode ParseTypeRef()
    {
        // segment - word separated by ::
        List<Token> segments = new(1);
        List<ObjAccessNode> genericArgs = new(0);

        while (true)
        {
            _state.ExpectTokenOrError(TokenType.Identifier);
            segments.Add(_state.CurrentToken);
            _state.NextToken();

            if (_state.CurrentTokenIs(TokenType.DoubleColon))
            {
                _state.NextToken();
            }
            else
            {
                break;
            }
        }

        if (_state.CurrentTokenIs(TokenType.LeftAngle))
        {
            _state.NextToken();

            while (!_state.CurrentTokenIs(TokenType.RightAngle))
            {
                genericArgs.Add(ParseTypeRef());
            }

            _state.NextToken(); // skip >
        }

        NamespaceName? namespaceName = null;

        if (segments.Count > 1)
        {
            // the segments of *only* the namespace name
            IEnumerable<Token> namespaceNameSegments = segments.TakeLast(1);
            Position start = segments.First().Location.Range.Start;
            Position end = segments.Last().Location.Range.End;
            Range range = new(start, end);
            FileLocation location = _state.CurrentToken.Location with
            {
                Range = range
            };
            namespaceName = new NamespaceName(String.Join("", namespaceNameSegments), location);
        }

        Token typeNameToken = segments.Last();
        return new ObjAccessNode(
            namespaceName,
            typeNameToken.Value.ToString(),
            genericArgs,
            typeNameToken.Location
        );
    }
}