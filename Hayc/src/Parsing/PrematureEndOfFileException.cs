namespace Hayc.Parsing;

public class PrematureEndOfFileException : Exception
{
    public PrematureEndOfFileException() : base("EOF reached prematurely.")
    {
    }
}