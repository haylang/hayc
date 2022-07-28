using HaycLib.Lexing;

namespace HaycLib.Parsing;

/// <summary>
/// Thrown when a syntax error occurs.
/// </summary>
public class ParsingException : Exception
{
    public ParsingException(string error, Token errorToken)
    {
        Error      = error;
        ErrorToken = errorToken;
    }

    /// <summary>
    /// The error text.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// The token that caused the error.
    /// </summary>
    public Token ErrorToken { get; }
}