namespace HaycLib.Lexing;

public enum TokenType
{
    #region Comments

    [TokenType("^\\/\\/.*")]              // Line
    [TokenType("^\\/\\*(.|\n)*\\*\\/")]   // Block
    Skip,

    #endregion
    
    #region Keywords
    
    [SimpleTokenType("namespace")]
    Namespace,
    
    [SimpleTokenType("struct")]
    Struct,
    
    [SimpleTokenType("import")]
    Import,
    
    [SimpleTokenType("extern")]
    Extern,
    
    [SimpleTokenType("return")]
    Return,
    
    [SimpleTokenType("private")]
    Modifier,
    
    #endregion

    #region Literals
    
    // Numbers
    [TokenType("^\"(.*)\"")]
    String,
    
    [TokenType("^\'.\'")]
    Character,
    
    [SimpleTokenType("true", "false")]
    Boolean,
    
    [TokenType("^[-+]?[0-9]*\\.[0-9]+")]
    Decimal,
    
    [TokenType("^[-+]?(0x|0b)?[0-9]+")]
    Integer,

    #endregion

    #region Operators

    [SimpleTokenType("->")]
    Arrow,
    
    [SimpleTokenType("&&")]
    And,
    
    [SimpleTokenType("||")]
    Or,
    
    [SimpleTokenType("::")]
    DoubleColon,
    
    [SimpleTokenType('=')]
    Assign,
    
    [SimpleTokenType("==")]
    Equal,
    
    [SimpleTokenType("!=")]
    NotEqual,
    
    [SimpleTokenType('@')]
    At,
    
    [SimpleTokenType('^')]
    Power,
    
    [SimpleTokenType('+')]
    Plus,
    
    [SimpleTokenType('-')]
    Minus,
    
    [SimpleTokenType('*')]
    Asterisk,
    
    [SimpleTokenType('/')]
    Slash,
    
    [SimpleTokenType(':')]
    Colon,
    
    [SimpleTokenType('?')]
    Question,
    
    [SimpleTokenType('&')]
    Ampersand,
    
    [SimpleTokenType(',')]
    Comma,
    
    [SimpleTokenType('.')]
    Dot,
    
    [SimpleTokenType(';')]
    Semicolon,
    
    #endregion
    
    #region Braces

    [SimpleTokenType('(')]
    LeftParen,
    
    [SimpleTokenType(')')]
    RightParen,
    
    [SimpleTokenType('{')]
    LeftBrace,
    
    [SimpleTokenType('}')]
    RightBrace,
    
    [SimpleTokenType('<')]
    LeftAngle,
    
    [SimpleTokenType('>')]
    RightAngle,

    #endregion
    
    [TokenType("^((\\p{L}|_)(\\p{L}|[0-9_])*)")]
    Identifier,
    
    /// <summary>
    /// This token type is special. It's reported for invalid tokens.
    /// Such tokens should be filtered out and not passed into parsers.
    /// </summary>
    Invalid = -1
}