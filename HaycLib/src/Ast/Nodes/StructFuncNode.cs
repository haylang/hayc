using System.Collections;
using HaycLib.Ast.DataObjects;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class StructFuncNode : FuncNode
{
    public StructFuncNode(
        string name,
        IEnumerable<VariableNode> parameters,
        IEnumerable<HayAttribute> attributes,
        TypeRefNode returnType,
        FileLocation location,
        TypeRefNode ownerStruct)
        : base(name, attributes, parameters, returnType, location)
    {
        OwnerStruct = ownerStruct;
    }
    
    /// <summary>
    /// The structure this function belongs to.
    /// </summary>
    public TypeRefNode OwnerStruct { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}