namespace HaycLib.Parsing;

/// <summary>
/// Thrown when the end of the file is reached.
/// </summary>
public class EndOfFileException : Exception
{
    public EndOfFileException()
        : base("The end of the file was reached.")
    {
    }
}