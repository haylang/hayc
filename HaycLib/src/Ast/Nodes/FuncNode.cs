using System.Collections;
using HaycLib.Ast.Data;
using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class FuncNode : AstNode
{
    public FuncNode(
        string name,
        IEnumerable<HayAttribute> attributes,
        IEnumerable<VariableNode> parameters,
        ObjAccessNode returnType,
        FileLocation location)
        : base(location)
    {
        Name       = name;
        Attributes = attributes;
        Parameters = parameters;
        ReturnType = returnType;
    }

    /// <summary>
    /// The name of the function.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The attributes of the function.
    /// </summary>
    public IEnumerable<HayAttribute> Attributes { get; }

    /// <summary>
    /// The params of the function.
    /// </summary>
    public IEnumerable<VariableNode> Parameters { get; }

    public ObjAccessNode ReturnType { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}