using Antlr4.Runtime;
using HaycLib.Location;
using Range = HaycLib.Location.Range;

namespace HaycLib.Reporting;

/// <summary>
/// An <see cref="IAntlrErrorListener{TSymbol}"/> for reporting errors
/// in a <see cref="MessageBatch"/>.
/// </summary>
public class MessageErrorListener : BaseErrorListener
{
    public MessageErrorListener(string filePath, MessageBatch messageBatch)
    {
        _filePath     = filePath;
        _messageBatch = messageBatch;
    }

    /// <summary>
    /// The path of the file.
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// The message batch.
    /// </summary>
    private MessageBatch _messageBatch;

    public override void SyntaxError(
        TextWriter output,
        IRecognizer recognizer,
        IToken offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        Position start = new(line, charPositionInLine);
        int tokenLength = offendingSymbol.Text.Length;
        Position end = tokenLength > 1
                           ? new Position(line, charPositionInLine + tokenLength)
                           : start;
        Range range = new(start, end);
        FileLocation location = new FileLocation(_filePath, range);
        _messageBatch.AddError($"Parsing: {msg}.", location);
    }
}