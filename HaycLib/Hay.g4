grammar Hay;

@header {
// CLI compliance attribute
#pragma warning disable CS3021
namespace HaycLib.Antlr;
}

@footer {
// CLI compliance attribute
#pragma warning restore CS3021
}

/*
 * Parser
 */

file
    : namespaceImport*
      namespaceDeclaration
      topLevelConstruct* EOF
    ;

// Namespace imports
namespaceImport
    : IMPORT name SEMICOLON
    ;

// Top-level constructs
namespaceDeclaration
    : NAMESPACE name SEMICOLON
    ;

topLevelConstruct
    : structDeclaration
    | functionDeclaration
    ;

structDeclaration
    : STRUCT structName=IDENTIFIER structBody
    ;

structBody
    : LBRACE structInitializer* fields=structField* RBRACE
    ;

structInitializer
    : INITIALIZER LPAREN parameters=parameterList RPAREN body=statementBlock
    ;

structField
    : typeName fieldName=IDENTIFIER SEMICOLON
    ;

functionDeclaration
    : FUNCTION (AT ownerStruct=name)? functionName=IDENTIFIER
        LPAREN parameters=parameterList RPAREN
        COLON returnType=typeName
        body=statementBlock
    ;

// Statements
statementBlock
    : LBRACE statement* RBRACE
    ;

statement
    : SEMICOLON                          #EmptyStatement
    | variableDeclaration SEMICOLON      #VariableDeclarationStatement
    | RETURN value=expression? SEMICOLON #ReturnStatement
    
    // Note: by standard, only function calls and variable assignments are allowed.
    // The compiler/interpreter must mark anything that is not a function call
    // nor a variable assignment as an error. This is done due to ANTLR limitations.
    | expression SEMICOLON  #ExpressionStatement
    ;
    
variableDeclaration
    : typeName variableName=IDENTIFIER (ASSIGN value=expression)
    ;

parameterList
    : (parameter (COMMA parameter)*)?
    ;

parameter
    : typeName paramName=IDENTIFIER SEMICOLON
    ;

// Expressions
expressionList
    : expression (COMMA expression)*
    ;

expression
    : number                                                    #NumberExpression
    | string                                                    #StringExpression
    | name                                                      #NameReferenceExpression
    | target=expression DOT child=IDENTIFIER                    #AccessExpression
    | target=expression LPAREN arguments=expressionList? RPAREN #FunctionCallExpression
    | target=expression ASSIGN expression                       #VariableAssignmentExpression
    ;

// Literals
typeName
    : name AMPERSAND?
    ;

name
    : parent=name DOUBLE_COLON child=name  #StaticAccessName
    | IDENTIFIER                           #MemberAccessName
    ;

number
    : DEC_INT    #DecimalInteger
    | HEX_INT    #HexInteger
    | DEC_DOUBLE #Double
    ;

string
    : STRING
    ;

/*
 * Lexer
 */
BLOCK_COMMENT : '/*' .*? '*/' -> skip;
LINE_COMMENT  : '//' ~[\r\n]* -> skip;
WS            : [ \n\t\r]+    -> skip;

// Keywords
IMPORT : 'import';
FUNCTION : 'function';
NAMESPACE : 'namespace';
STRUCT : 'struct';
INITIALIZER : 'initializer';
RETURN : 'return';

// Literals
STRING : '"' .*? '"';
DEC_INT : Digit+;
HEX_INT : '0x' HexDigit+;
DEC_DOUBLE : Digit* '.' Digit+;

// Punctuation
AMPERSAND : '&';
ASSIGN : '=';
LPAREN : '(';
RPAREN : ')';
LBRACE : '{';
RBRACE : '}';
DOUBLE_COLON : '::';
SEMICOLON : ';';
COLON : ':';
DOT : '.';
COMMA : ',';
AT : '@';

// Misc.
IDENTIFIER : IdentifierChar+;

// Fragments
fragment IdentifierChar : [a-zA-Z0-9_];
fragment Digit : [0-9];
fragment HexDigit : Digit | [A-Fa-f];