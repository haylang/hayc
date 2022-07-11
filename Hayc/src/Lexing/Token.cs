using Hayc.Common;
using Hayc.Common.Location;

namespace Hayc.Lexing;

public record Token(TokenType Type, string Value, FileLocation File)
{
    public override string ToString() => $"{Type,-20}{Value,-20}";
}