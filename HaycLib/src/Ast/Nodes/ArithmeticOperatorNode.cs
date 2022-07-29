using HaycLib.Ast.Data;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class ArithmeticOperatorNode : BinaryOperatorNode
{
    public ArithmeticOperatorNode(
        ArithmeticOperatorType type,
        AstNode left,
        AstNode right,
        FileLocation location)
        : base(left, right, location)
    {
        Type = type;
    }

    /// <summary>
    /// The type of arithmetic operator this is.
    /// </summary>
    public ArithmeticOperatorType Type { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}