using HaycLib.Lexing;
using HaycLib.Location;
using HaycLib.Reporting;

namespace HaycLib.Parsing;

public sealed class ParserState
{
    public ParserState(List<Token> tokens)
    {
        _tokens    = tokens;
        _snapshots = new Stack<ParserStateSnapshot>();
        _snapshots.Push(new ParserStateSnapshot(0));
    }

    /// <summary>
    /// The tokens.
    /// </summary>
    private readonly List<Token> _tokens;

    /// <summary>
    /// The parser snapshots.
    /// </summary>
    private readonly Stack<ParserStateSnapshot> _snapshots;

    /// <summary>
    /// Retrieves the current token.
    /// </summary>
    public Token CurrentToken => PeekToken(0);

    /// <summary>
    /// The current parser snapshot.
    /// </summary>
    public ParserStateSnapshot CurrentSnapshot => _snapshots.Peek();

    /// <summary>
    /// Whether EOF has been reached.
    /// </summary>
    public bool ReachedEndOfFile => CurrentSnapshot.Index >= _tokens.Count;

    /// <summary>
    /// Begins a new parser snapshot.
    /// </summary>
    public void BeginNewSnapshot()
    {
        _snapshots.Push(new ParserStateSnapshot(CurrentSnapshot.Index));
    }

    /// <summary>
    /// Reverts to the previous snapshot.
    /// </summary>
    public void RevertToPreviousSnapshot()
    {
        if (_snapshots.Count == 1)
        {
            throw new InvalidOperationException("There is only 1 snapshot.");
        }

        _snapshots.Pop();
    }

    /// <summary>
    /// Shorthand for <see cref="CurrentToken"/>.Type == tokenType.
    /// </summary>
    public bool CurrentTokenIs(TokenType tokenType)
    {
        return CurrentToken.Type == tokenType;
    }

    /// <summary>
    /// Peeks the next token.
    /// </summary>
    public Token PeekToken(int with = 1)
    {
        int index = Math.Clamp(CurrentSnapshot.Index + with, 0, _tokens.Count - 1);
        return _tokens[index];
    }

    /// <summary>
    /// Advances the token position.
    /// </summary>
    public void NextToken()
    {
        CurrentSnapshot.Index++;
    }

    /// <summary>
    /// Skips tokens until it reaches the given token.
    /// It consumes the given token, leaving the parser on the next one.
    /// </summary>
    public void SkipUntil(TokenType tokenType, int max = -1)
    {
        while (max-- != 0)
        {
            if (CurrentToken.Type == tokenType)
            {
                NextToken();
                return;
            }

            NextToken();
        }
    }

    /// <summary>
    /// Advances to the next token while the current token type matches the given.
    /// </summary>
    public void SkipOfType(TokenType tokenType)
    {
        while (CurrentTokenIs(tokenType))
        {
            NextToken();
        }
    }

    /// <summary>
    /// Expects a token, reports an error otherwise.
    /// </summary>
    public void ExpectTokenOrError(TokenType tokenType)
    {
        if (CurrentToken.Type == tokenType)
        {
            return;
        }

        Error($"Expected {tokenType}, got '{CurrentToken.Value}'.", CurrentToken);
    }

    /// <summary>
    /// Adds an error.
    /// Routes the call to the current snapshot's message batch.
    /// </summary>
    public void Error(string message, Token token)
        => CurrentSnapshot.MessageBatch.AddError(message, token.Location);

    /// <summary>
    /// Adds an error at the current token.
    /// Routes the call to the current snapshot's message batch.
    /// </summary>
    public void ErrCurrentToken(string message)
        => CurrentSnapshot.MessageBatch.AddError(message, CurrentToken.Location);

    /// <summary>
    /// Retrieves all messages from all snapshots.
    /// </summary>
    public IEnumerable<Message> GetAllMessages()
    {
        List<Message> messages = new();
        foreach (ParserStateSnapshot snapshot in _snapshots)
        {
            messages.AddRange(snapshot.MessageBatch.Messages);
        }
        return messages;
    }

    /// <summary>
    /// Checks whether there is a token at that index.
    /// </summary>
    private void BoundsExceptionGuard(int index)
    {
        if (ReachedEndOfFile)
        {
            throw new EndOfFileException();
        }
    }
}