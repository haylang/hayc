using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class VariableNode : AstNode
{
    public VariableNode(string name, TypeRefNode type, FileLocation location)
        : base(location)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type of the variable.
    /// </summary>
    public TypeRefNode Type { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}