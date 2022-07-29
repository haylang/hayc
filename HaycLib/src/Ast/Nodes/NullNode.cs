using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class NullNode : AstNode, IExpression
{
    public NullNode(FileLocation location) : base(location)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}