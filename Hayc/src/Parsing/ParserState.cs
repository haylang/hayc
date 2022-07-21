using Hayc.Lexing;

namespace Hayc.Parsing;

/// <summary>
/// The state of the parser.
/// </summary>
public class ParserState
{
    public ParserState(Token[] tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// The current token position.
    /// </summary>
    public int TokenPosition
    {
        get => _tokenPosition;
        set
        {
            if (value < 0
                || value > _tokens.Length)
            {
                throw new IndexOutOfRangeException();
            }
            
            _tokenPosition = value;
        }
    }

    /// <summary>
    /// Whether the parser has reached the end of the file.
    /// </summary>
    public bool ReachedEndOfFile => TokenPosition >= _tokens.Length;

    /// <summary>
    /// The tokens of the parser state.
    /// </summary>
    private readonly Token[] _tokens;

    /// <summary>
    /// Backing field for <see cref="TokenPosition"/>.
    /// </summary>
    private int _tokenPosition;

    /// <summary>
    /// Peeks the next token.
    /// </summary>
    public Token PeekToken(int with = 1)
    {
        if (TokenPosition + with >= _tokens.Length)
        {
            throw new PrematureEndOfFileException();
        }

        return _tokens[TokenPosition + with];
    }

    /// <summary>
    /// Moves the token position by one and returns the token at the new position.
    /// </summary>
    /// <remarks>
    /// The first time this method is called, the very first token will be returned,
    /// and only then will the position incrementing begin.
    /// </remarks>
    public Token NextToken()
    {
        if (TokenPosition + 1 >= _tokens.Length)
        {
            throw new PrematureEndOfFileException();
        }

        return _tokens[TokenPosition++];
    }
}