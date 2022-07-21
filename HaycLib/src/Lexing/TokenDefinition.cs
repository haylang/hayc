using System.Text.RegularExpressions;

namespace HaycLib.Lexing;

public static class TokenDefinition
{
    /// <summary>
    /// The tokens in the language.
    /// </summary>
    public static readonly (TokenType Type, Regex Regex)[] Definitions =
    {
        (TokenType.Skip, new Regex("^\\/\\/.*", RegexOptions.Compiled)),
        (TokenType.Skip, new Regex("^\\/\\*(.|\n)*\\*\\/", RegexOptions.Compiled | RegexOptions.Multiline)),

        (TokenType.Struct, new Regex("^struct\\b", RegexOptions.Compiled)),
        (TokenType.Initializer, new Regex("^initializer\\b", RegexOptions.Compiled)),
        (TokenType.Function, new Regex("^function\\b", RegexOptions.Compiled)),
        (TokenType.If, new Regex("^if\\b", RegexOptions.Compiled)),
        (TokenType.Else, new Regex("^else\\b", RegexOptions.Compiled)),
        (TokenType.While, new Regex("^while\\b", RegexOptions.Compiled)),
        (TokenType.Return, new Regex("^return\\b", RegexOptions.Compiled)),
        (TokenType.Do, new Regex("^do\\b", RegexOptions.Compiled)),
        (TokenType.This, new Regex("^this\\b", RegexOptions.Compiled)),
        (TokenType.Include, new Regex("^include\\b", RegexOptions.Compiled)),
        
        (TokenType.String, new Regex("^(?<=\")(.*)(?=\")", RegexOptions.Compiled)),
        (TokenType.Character, new Regex("^(?<=')\\\\?.(?=')", RegexOptions.Compiled)),
        (TokenType.Boolean, new Regex("^(true|false)\\b", RegexOptions.Compiled)),
        (TokenType.Decimal, new Regex("^[-+]?[0-9]*\\.[0-9]+", RegexOptions.Compiled)),
        (TokenType.Integer, new Regex("^[-+]?(0x|0b)?[0-9]+", RegexOptions.Compiled)),
        
        new(TokenType.Arrow, new Regex("^->", RegexOptions.Compiled)),
        new(TokenType.And, new Regex("^&&", RegexOptions.Compiled)),
        new(TokenType.Or, new Regex("^\\|\\|", RegexOptions.Compiled)),
        new(TokenType.DoubleColon, new Regex("^::", RegexOptions.Compiled)),
        new(TokenType.Assign, new Regex("^=")),
        new(TokenType.Equal, new Regex("^==")),
        new(TokenType.NotEqual, new Regex("^!=")),
        new(TokenType.At, new Regex("^@")),
        new(TokenType.Plus, new Regex("^\\+", RegexOptions.Compiled)),
        new(TokenType.Minus, new Regex("^-", RegexOptions.Compiled)),
        new(TokenType.Asterisk, new Regex("^\\*", RegexOptions.Compiled)),
        new(TokenType.Slash, new Regex("^\\/", RegexOptions.Compiled)),
        new(TokenType.Colon, new Regex("^:", RegexOptions.Compiled)),
        new(TokenType.Question, new Regex("^\\?", RegexOptions.Compiled)),
        new(TokenType.Ampersand, new Regex("^&", RegexOptions.Compiled)),
        new(TokenType.Comma, new Regex("^,", RegexOptions.Compiled)),
        new(TokenType.Dot, new Regex("^\\.", RegexOptions.Compiled)),
        new(TokenType.Semicolon, new Regex("^;", RegexOptions.Compiled)),

        new(TokenType.LeftParen, new Regex("^\\(", RegexOptions.Compiled)),
        new(TokenType.RightParen, new Regex("^\\)", RegexOptions.Compiled)),
        new(TokenType.LeftBrace, new Regex("^\\{", RegexOptions.Compiled)),
        new(TokenType.RightBrace, new Regex("^\\}", RegexOptions.Compiled)),
        new(TokenType.LeftAngle, new Regex("^<", RegexOptions.Compiled)),
        new(TokenType.RightAngle, new Regex("^>", RegexOptions.Compiled)),
        
        new(TokenType.Identifier, new Regex("^((\\p{L}|_)(\\p{L}|[0-9_])*)", RegexOptions.Compiled))
    };
}