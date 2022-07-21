namespace HaycLib.Reporting;

/// <summary>
/// Numerical message error codes.
/// </summary>
public enum MessageId
{
    /*
     * Lexing
     * 1XXX
     */
    InvalidCharacter = 1000,
    
    /*
     * Parsing
     * 3XXX
     */
    
    /*
     * Semantic analysis
     * 5XXX
     */
    
    /*
     * Misc error
     */
    Miscellaneous = 9000
}