namespace Hayc.Common.Location;

/// <summary>
/// Represents a range of characters in a file.
/// </summary>
public record Range(Position Start, Position End);