using HaycLib.Location;
using Range = HaycLib.Location.Range;

namespace HaycLib.Lexing;

/// <summary>
/// A lexeme in the source file.
/// </summary>
public sealed class Token
{
    public Token(TokenType type, FileLocation location, string fileString, int start, int length)
    {
        Type           = type;
        Location       = location;
        _refFileString = fileString;
        _startIndex    = start;
        _length        = length;
    }

    /// <summary>
    /// The type of the token.
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// The position of the token.
    /// </summary>
    public FileLocation Location { get; }

    /// <summary>
    /// The value of the token.
    /// </summary>
    public ReadOnlySpan<char> Value => _refFileString.Substring(_startIndex, _length);

    public override string ToString()
    {
        return $"Token{{{Type}}}({Value})";
    }

    /// <summary>
    /// Reference to the file content string.
    /// </summary>
    private readonly string _refFileString;

    /// <summary>
    /// The starting index in the <see cref="_refFileString"/>.
    /// </summary>
    private readonly int _startIndex;

    /// <summary>
    /// The length of the value of the token.
    /// </summary>
    private readonly int _length;
}