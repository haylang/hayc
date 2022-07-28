using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class StructDestructorNode : AstNode
{
    public StructDestructorNode(
        BlockNode body,
        FileLocation sourceLocation)
        : base(sourceLocation)
    {
        Body = body;
    }

    /// <summary>
    /// The body of the destructor.
    /// </summary>
    public BlockNode Body { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}