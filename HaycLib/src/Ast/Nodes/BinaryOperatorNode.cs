using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public abstract class BinaryOperatorNode : AstNode, IExpression
{
    protected BinaryOperatorNode(AstNode left, AstNode right, FileLocation sourceLocation)
        : base(sourceLocation)
    {
        Left  = left;
        Right = right;
    }

    /// <summary>
    /// The left operand of the operator.
    /// </summary>
    public AstNode Left { get; }

    /// <summary>
    /// The right operand of the operator.
    /// </summary>
    public AstNode Right { get; }
}