using System.Text.RegularExpressions;

namespace HaycLib.Lexing;

/// <summary>
/// A special kind of <see cref="TokenTypeAttribute"/> that wraps the passed
/// keywords in a regex of the following format: ^(keyw1|key2) with escaping.
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