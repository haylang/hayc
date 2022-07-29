using HaycLib.Lexing;

namespace HaycLib.Ast.Data;

public enum ArithmeticOperatorType
{
    Add = TokenType.Plus,
    Sub = TokenType.Minus,
    Mul = TokenType.Asterisk,
    Div = TokenType.Slash,
    Mod = TokenType.Percent
}