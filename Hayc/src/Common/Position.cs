namespace Hayc.Common;

/// <summary>
/// Represents a position within a file.
/// </summary>
public readonly struct Position
{
    public bool Equals(Position other)
    {
        return Line == other.Line
               && Column == other.Column;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Line, Column);
    }

    public static readonly Position FileStart = new(1, 1);

    public Position(int line, int column)
    {
        Line   = line;
        Column = column;
    }

    /// <summary>
    /// The line.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// The column.
    /// </summary>
    public int Column { get; }

    public static Range operator +(Position start, Position end)
    {
        return new Range(start, end);
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Line == right.Line && left.Column == right.Column;
    }

    public static bool operator !=(Position left, Position right)
    {
        return !(left == right);
    }


    public override string ToString()
    {
        return $"Ln {Line} Col {Column}";
    }
}