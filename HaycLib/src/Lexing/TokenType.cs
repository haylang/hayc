namespace HaycLib.Lexing;

public enum TokenType
{
    Invalid,
    Skip,
    
    // Keywords
    Struct,
    If,
    Else,
    While,
    Return,
    Do,
    This,
    Include,
    Initializer,
    Function,
    
    // Literals
    Identifier,
    String,
    Character,
    Boolean,
    Integer,
    Decimal,
    Null,
    
    // Operators
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    Equal,
    NotEqual,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    And,
    Or,
    Not,
    Assign,
    At,
    Colon,
    DoubleColon,
    Asterisk,
    Arrow,
    Slash,
    Question,
    Comma,
    Dot,
    Semicolon,
    Ampersand,
    
    // Braces
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftAngle,
    RightAngle
}