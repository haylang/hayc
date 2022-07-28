using HaycLib.Reporting;

namespace HaycLib.Parsing;

/// <summary>
/// Used for backtracking the parser state.
/// </summary>
public sealed class ParserStateSnapshot
{
    public ParserStateSnapshot(int index)
    {
        Index        = index;
        MessageBatch = new MessageBatch();
    }

    /// <summary>
    /// The current token index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The message batch of the snapshot.
    /// </summary>
    public MessageBatch MessageBatch { get; }
}