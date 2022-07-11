namespace Hayc.Common.Location;

/// <summary>
/// Represents a position within a file.
/// </summary>
public record Position(int Line, int Column)
{
    public static readonly Position FileStart = new(1, 1);
    
    public static Range operator +(Position start, Position end)
    {
        return new Range(start, end);
    }
}