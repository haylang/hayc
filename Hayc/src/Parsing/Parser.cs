using Hayc.Lexing;

namespace Hayc.Parsing;

/// <summary>
/// Parses a source file.
/// </summary>
public class Parser
{
    public Parser(Token[] tokens)
    {
        _state = new ParserState(tokens);
    }

    /// <summary>
    /// The parser state.
    /// </summary>
    private readonly ParserState _state;

    public void Parse()
    {
        
    }
}