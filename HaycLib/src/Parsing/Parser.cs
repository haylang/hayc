using HaycLib.Ast;
using HaycLib.Lexing;
using HaycLib.Reporting;

namespace HaycLib.Parsing;

/// <summary>
/// Parses a source file.
/// </summary>
public class Parser
{
    public Parser(MessageBatch messages, Token[] tokens)
    {
        _messages = messages;
        _state    = new ParserState(tokens);
    }

    /// <summary>
    /// The compilation message batch.
    /// </summary>
    private readonly MessageBatch _messages;

    /// <summary>
    /// The parser state.
    /// </summary>
    private readonly ParserState _state;

    public FileNode Parse()
    {
        // TODO: Implement parser
        return new FileNode();
    }
}