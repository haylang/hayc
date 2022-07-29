using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class DecimalNode : AstNode, IExpression
{
    public DecimalNode(double value, FileLocation sourceLocation) : base(sourceLocation)
    {
        Value = value;
    }
    
    /// <summary>
    /// The value of the decimal.
    /// </summary>
    public double Value { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}