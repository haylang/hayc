namespace Hayc.Location;

/// <summary>
/// Represents a range within a file.
/// </summary>
public record FileLocation(string Path, Range Range);