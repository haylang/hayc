using Hayc.Common.Location;

namespace Hayc.Common.Reporting;

/// <summary>
/// Represents a message during compilation.
/// </summary>
/// <param name="Content">The text of the message.</param>
/// <param name="Location">The location of the message.</param>
/// <param name="Id">The numerical ID of the message.</param>
public record Message(string Content, FileLocation Location, MessageId Id, MessageSeverity Severity);