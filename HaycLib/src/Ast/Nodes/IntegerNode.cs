using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class IntegerNode : AstNode, IExpression
{
    public IntegerNode(long value, FileLocation sourceLocation) : base(sourceLocation)
    {
        Value = value;
    }
    
    /// <summary>
    /// The value of the integer.
    /// </summary>
    public long Value { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}