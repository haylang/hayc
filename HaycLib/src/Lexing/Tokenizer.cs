using System.Text;
using System.Text.RegularExpressions;
using HaycLib.Extensions;
using HaycLib.Location;
using Range = HaycLib.Location.Range;

namespace HaycLib.Lexing;

/// <summary>
/// Performs tokenization on files.
/// </summary>
public sealed class Tokenizer
{
    public Tokenizer(string fileName, string sourceText)
    {
        _fileName           = fileName;
        _originalSourceText = _workingSourceText = sourceText;
    }

    /// <summary>
    /// The name of the file being tokenized.
    /// </summary>
    private readonly string _fileName;

    /// <summary>
    /// The source to tokenize.
    /// </summary>
    private readonly string _originalSourceText;

    /// <summary>
    /// The source to tokenize.
    /// </summary>
    private string _workingSourceText;

    /// <summary>
    /// Tokenizes the input
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Token> Tokenize()
    {
        List<Token> tokens = new();

        Token? next;
        while ((next = MatchNext()) != null)
        {
            tokens.Add(next);
        }

        return tokens;
    }

    /// <summary>
    /// Retrieves the next token.
    /// </summary>
    /// <returns>The next token or null if EOF is reached.</returns>
    private Token? MatchNext()
    {
        // Whitespaces in the beginning will obstruct the regex patterns
        _workingSourceText = _workingSourceText.TrimStart();

        if (_workingSourceText.Length == 0)
        {
            return null;
        }

        // This is the starting index of the following token
        int startIndex = _originalSourceText.Length - _workingSourceText.Length;
        Position tokenStart = GetPositionByIndex(startIndex);

        foreach (TokenType tokenType in Enum.GetValues<TokenType>())
        {
            if (tokenType == TokenType.Invalid)
            {
                continue;
            }

            foreach (Regex regex in tokenType.GetRegexes())
            {
                Match match = regex.Match(_workingSourceText);
                if (!match.Success)
                {
                    continue;
                }
                int tokenValueLength = match.Length;

                // Obviously we don't want to keep matching one token infinitely
                _workingSourceText = _workingSourceText[tokenValueLength ..];

                if (tokenType == TokenType.Skip)
                {
                    // This is a comment.
                    // Restart!
                    return MatchNext();
                }

                Position tokenEnd = GetPositionByIndex(startIndex + tokenValueLength);
                Range tokenRange = new(tokenStart, tokenEnd);
                FileLocation location = new(_fileName, tokenRange);

                return new Token(tokenType, location, _originalSourceText, startIndex, tokenValueLength);
            }
        }

        // If the above foreach didn't return, this is an invalid token.
        _workingSourceText = _workingSourceText[1 ..];
        return new Token(
            TokenType.Invalid,
            new FileLocation(_fileName, new Range(tokenStart, GetPositionByIndex(startIndex + 1))),
            _originalSourceText,
            startIndex,
            1
        );
    }

    /// <summary>
    /// Retrieves the position of the cursor in the source string by an index.
    /// </summary>
    private Position GetPositionByIndex(int index)
    {
        ReadOnlySpan<char> text = _originalSourceText.AsSpan()[.. index];
        ReadOnlySpan<char>.Enumerator e = text.GetEnumerator();
        int row = 0, col = 0, n = 0;
        while (e.MoveNext())
        {
            if (e.Current == '\n')
            {
                row++;
                col = n;
            }
            n++;
        }
        return new Position(row + 1, index - col);
    }
}