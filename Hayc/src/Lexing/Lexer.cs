using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Hayc.Common;
using Hayc.Common.Location;
using Hayc.Common.Reporting;

namespace Hayc.Lexing;

/// <summary>
/// Performs lexical analysis on a string of characters.
/// </summary>
public sealed class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["if"]      = TokenType.If,
        ["else"]    = TokenType.Else,
        ["while"]   = TokenType.While,
        ["do"]      = TokenType.Do,
        ["return"]  = TokenType.Return,
        ["true"]    = TokenType.Boolean,
        ["false"]   = TokenType.Boolean,
        ["null"]    = TokenType.Null,
        ["this"]    = TokenType.This,
        ["include"] = TokenType.Include,
        ["struct"]  = TokenType.Struct,
    };

    public Lexer(string fileName, string parseContent)
    {
        _fileName        = fileName;
        _startingContent = parseContent;
        _parseContent    = new StringBuilder(parseContent);

        Messages = new MessageBatch();
    }

    public MessageBatch Messages { get; }
    
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
                    Messages.AddError($"Invalid character: {current.Value}", current.File, MessageId.InvalidCharacter);
                    continue;
                default:
                    tokens.Add(current);
                    break;
            }
        }

        return tokens.ToArray();
    }

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