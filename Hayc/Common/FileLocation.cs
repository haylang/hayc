namespace Hayc.Common;

/// <summary>
/// Represents a range within a file.
/// </summary>
public record FileLocation(string Path, Range Range)
{
    public override string ToString() => $"{Path} at {Range}";
}