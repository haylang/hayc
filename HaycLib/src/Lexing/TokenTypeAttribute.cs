using System.Text.RegularExpressions;

namespace HaycLib.Lexing;

/// <summary>
/// Enum values in <see cref="TokenType"/> marked with this attribute
/// are considered tokens by the lexer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TokenTypeAttribute : Attribute
{
    public TokenTypeAttribute(string regexPattern, bool multiline = false)
    {
        Regex = new Regex(
            regexPattern,
            RegexOptions.Compiled
            | (multiline ? RegexOptions.Multiline : RegexOptions.None)
        );
    }

    /// <summary>
    /// The regex for this token type.
    /// </summary>
    public Regex Regex { get; }
}