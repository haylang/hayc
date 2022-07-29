using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class VariableNode : AstNode
{
    public VariableNode(
        string name,
        ObjAccessNode type,
        FileLocation location,
        AstNode? defaultValue)
        : base(location)
    {
        Name         = name;
        Type         = type;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type of the variable.
    /// </summary>
    public ObjAccessNode Type { get; }

    /// <summary>
    /// The default value of the variable.
    /// </summary>
    public AstNode? DefaultValue { get; set; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}