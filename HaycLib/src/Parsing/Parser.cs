using System.Text;
using HaycLib.Ast.DataObjects;
using HaycLib.Ast.Nodes;
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
               && this.IsImport())
        {
            imports.Add(ParseImport());
        }

        NamespaceName namespaceName;
        if (!_state.ReachedEndOfFile
            && this.IsNamespaceStatement())
        {
            namespaceName = ParseNamespaceStatement();
        }
        else
        {
            namespaceName = new NamespaceName("???", _state.CurrentToken.Location);
            _state.CurrentTokenErr("Expected namespace declaration.");
        }

        FileNode fileNode = new(namespaceName, imports);
        return fileNode;
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
}