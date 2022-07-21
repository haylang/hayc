using HaycLib.Location;

namespace HaycLib.Reporting;

/// <summary>
/// Stores messages.
/// </summary>
public sealed class MessageBatch
{
    public MessageBatch()
    {
        _messages = new List<Message>();
    }

    /// <summary>
    /// The internal list of messages.
    /// </summary>
    private readonly List<Message> _messages;

    /// <summary>
    /// The list of messages.
    /// </summary>
    public IEnumerable<Message> Messages => _messages;

    /// <summary>
    /// Only retrieves the messages of the specified severity.
    /// </summary>
    /// <param name="severity">The required severity.</param>
    public IEnumerable<Message> Filter(MessageSeverity severity) => _messages.Where(m => m.Severity == severity);

    /// <summary>
    /// Adds an error.
    /// </summary>
    public void AddError(string content, FileLocation location, MessageId id)
        => _messages.Add(new Message(content, location, id, MessageSeverity.Error));

    /// <summary>
    /// Adds a warning.
    /// </summary>
    public void AddWarn(string content, FileLocation location, MessageId id)
        => _messages.Add(new Message(content, location, id, MessageSeverity.Warning));
    
    /// <summary>
    /// Adds a note.
    /// </summary>
    public void AddInfo(string content, FileLocation location, MessageId id)
        => _messages.Add(new Message(content, location, id, MessageSeverity.Info));
}