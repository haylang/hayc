using System.Text;
using System.Text.RegularExpressions;
using HaycLib.Location;
using HaycLib.Reporting;

namespace HaycLib.Lexing;

/// <summary>
/// Performs lexical analysis on a string of characters.
/// </summary>
public sealed class Lexer
{
    public Lexer(MessageBatch messageBatch, string fileName, string parseContent)
    {
        _messageBatch    = messageBatch;
        _fileName        = fileName;
        _startingContent = parseContent;
        _parseContent    = new StringBuilder(parseContent);
    }

    /// <summary>
    /// The messages of the lexer.
    /// </summary>
    private readonly MessageBatch _messageBatch;

    /// <summary>
    /// The current file position.
    /// </summary>
    private Position CurrentPosition
    {
        get
        {
            int currentPos = _startingContent.Length - _parseContent.Length;
            int line = 1;
            int column = 1;
            for (int i = 0; i < currentPos; ++i)
            {
                switch (_startingContent[i])
                {
                    case '\r':
                        continue;
                    case '\n':
                        line++;
                        column = 1;
                        break;
                    default:
                        column++;
                        break;
                }
            }

            return new Position(line, column);
        }
    }

    /// <summary>
    /// The name of the file.
    /// </summary>
    private readonly string _fileName;

    /// <summary>
    /// The string to read from.
    /// </summary>
    private readonly string _startingContent;

    /// <summary>
    /// The stream to read from.
    /// </summary>
    private readonly StringBuilder _parseContent;

    /// <summary>
    /// Tokenizes the entire stream input.
    /// </summary>
    /// <returns>An array of the tokens.</returns>
    public Token[] Lex()
    {
        List<Token> tokens = new();

        for (Token? current; (current = Match()) != null;)
        {
            switch (current.Type)
            {
                case TokenType.Skip:
                    continue;
                case TokenType.Invalid:
                    _messageBatch.AddError(
                        $"Invalid character: {current.Value}",
                        current.File,
                        MessageId.InvalidCharacter
                    );
                    continue;
                default:
                    tokens.Add(current);
                    break;
            }
        }

        return tokens.ToArray();
    }

    /// <summary>
    /// Attempts to find a token to match the current string to.
    /// </summary>
    /// <returns></returns>
    private Token? Match()
    {
        string useString = _parseContent.ToString();

        string trimmed = useString.TrimStart();
        int leadingSpaces = useString.Length - trimmed.Length;
        useString = trimmed;

        if (useString.Length == 0)
        {
            return null;
        }

        Position tokenStart = CurrentPosition;

        foreach ((TokenType Type, Regex Regex) definition in TokenDefinition.Definitions)
        {
            Match match = definition.Regex.Match(useString);
            if (match.Success)
            {
                // Don't match the same token again
                _parseContent.Remove(0, leadingSpaces + match.Length);

                Position tokenEnd = CurrentPosition;

                return new Token(
                    definition.Type,
                    match.Value,
                    new FileLocation(_fileName, tokenStart + tokenEnd)
                );
            }
        }

        _parseContent.Remove(0, leadingSpaces + 1);
        return new Token(
            TokenType.Invalid,
            useString[0].ToString(),
            new FileLocation(_fileName, tokenStart + tokenStart)
        );
    }
}