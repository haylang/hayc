using System.Collections;
using HaycLib.Ast.Data;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class StructFuncNode : FuncNode
{
    public StructFuncNode(
        string name,
        BlockNode body,
        IEnumerable<HayAttribute> attributes,
        IEnumerable<VariableNode> parameters,
        ObjAccessNode returnType,
        FileLocation location,
        ObjAccessNode ownerStruct)
        : base(name, body, attributes, parameters, returnType, location)
    {
        OwnerStruct = ownerStruct;
    }
    
    /// <summary>
    /// The structure this function belongs to.
    /// </summary>
    public ObjAccessNode OwnerStruct { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}