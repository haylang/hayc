namespace HaycLib.Ast.Interfaces;

/// <summary>
/// Nodes that inherit this interface are unary operators.
/// Inherently, they are also expressions.
/// </summary>
public interface IUnaryOperator<T> : IExpression
    where T : IExpression
{
}