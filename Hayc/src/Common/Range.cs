namespace Hayc.Common;

/// <summary>
/// Represents a range of characters in a file.
/// </summary>
public readonly struct Range
{
    public Range(Position start, Position end)
    {
        Start = start;
        End   = end;
    }

    public Range(Position singlePosition) : this(singlePosition, singlePosition)
    {
    }
    
    /// <summary>
    /// The start of the range.
    /// </summary>
    public Position Start { get; }
    
    /// <summary>
    /// The end of the range.
    /// </summary>
    public Position End { get; }
    
    /// <summary>
    /// Whether the range is a single character.
    /// </summary>
    public bool IsSingleCharacter => Start == End || Start.Line == End.Line && Start.Column == End.Column - 1;

    public override string ToString()
    {
        return IsSingleCharacter ? Start.ToString() : $"from {Start} to {End}";
    }
}