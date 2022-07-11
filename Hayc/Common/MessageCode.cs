namespace Hayc.Common;

/// <summary>
/// Numerical message error codes.
/// </summary>
public enum MessageId
{
    /*
     * Lexing
     * 1XXXX
     */
    InvalidCharacter = 10001,

    /*
     * Parsing
     * 2XXXX
     */
    ExpectedToken                               = 20001,
    NoNamespaceName                             = 20002,
    ExpectedTopLevelConstruct                   = 20003,
    ExpectedTypeName                            = 20004,
    ExpectedListSeparator                       = 20005,
    ExpectedStatement                           = 20006,
    CannotDeclareUninitializedImmutableVariable = 20007,
    ExpectedExpression                          = 20008,
    PrematureEndOfFile                          = 20009,
    ExpectedObjectReference                     = 20010,
    ExpectedMemberName                          = 20011,
    AccessNonObjectAttempt                      = 20012,
}