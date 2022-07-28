namespace HaycLib.Parsing;

/// <summary>
/// Describes how the parser should try to recover from an error token.
/// </summary>
public enum ParseRecoverMode
{
    /// <summary>
    /// Uses <see cref="UntilBrace"/> if an opening brace is found,
    /// otherwise uses <see cref="UntilSemicolon"/>.
    /// </summary>
    Detect,
    
    /// <summary>
    /// Skips tokens until a brace.
    /// This does *not* consume the brace.
    /// </summary>
    UntilBrace,
    
    /// <summary>
    /// Skips tokens until a semicolon.
    /// This consumes the semicolon.
    /// </summary>
    UntilSemicolon
}