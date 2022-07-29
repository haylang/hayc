using System.Collections;
using HaycLib.Ast.Data;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class StructNode : AstNode
{
    public StructNode(
        string name,
        StructBody body,
        IEnumerable<HayAttribute> attributes,
        FileLocation location)
        : base(location)
    {
        Name       = name;
        Body       = body;
        Attributes = attributes;
    }

    /// <summary>
    /// The name of the structure.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The body of the structure.
    /// </summary>
    public StructBody Body { get; }

    /// <summary>
    /// The attributes on this struct.
    /// </summary>
    public IEnumerable<HayAttribute> Attributes { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}