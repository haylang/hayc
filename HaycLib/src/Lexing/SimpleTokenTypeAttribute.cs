using System.Text.RegularExpressions;

namespace HaycLib.Lexing;

/// <summary>
/// A special kind of <see cref="TokenTypeAttribute"/> that wraps the passed
/// values in a regex that will match them as alternatives. All passed inputs
/// are escaped.
/// </summary>
public class SimpleTokenTypeAttribute : TokenTypeAttribute
{
    public SimpleTokenTypeAttribute(params string[] words)
        : base($"^({String.Join('|', words.Select(Regex.Escape))})")
    {
    }

    public SimpleTokenTypeAttribute(string word)
        : base($"^({Regex.Escape(word)})")
    {
    }

    public SimpleTokenTypeAttribute(char symbol)
        : this(symbol.ToString())
    {
    }
}