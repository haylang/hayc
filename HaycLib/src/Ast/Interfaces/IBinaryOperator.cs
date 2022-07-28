namespace HaycLib.Ast.Interfaces;

/// <summary>
/// Nodes that inherit this interface are binary operators.
/// Inherently, they are also expressions.
/// </summary>
public interface IBinaryOperator<TLeft, TRight> : IExpression
    where TLeft : IExpression
    where TRight : IExpression
{
}