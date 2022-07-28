using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class StructInitializerNode : AstNode
{
    public StructInitializerNode(
        BlockNode body,
        List<VariableNode> parameters,
        FileLocation sourceLocation)
        : base(sourceLocation)
    {
        Body       = body;
        Parameters = parameters;
    }

    /// <summary>
    /// The body of the initializer.
    /// </summary>
    public BlockNode Body { get; }

    /// <summary>
    /// The parameters of the initializer.
    /// </summary>
    public List<VariableNode> Parameters { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}