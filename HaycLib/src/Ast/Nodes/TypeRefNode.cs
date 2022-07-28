using System.Collections;
using HaycLib.Ast.DataObjects;
using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class TypeRefNode : AstNode, IExpression
{
    public TypeRefNode(
        NamespaceName? namespaceName,
        string typeName,
        IEnumerable<TypeRefNode> genericArguments,
        FileLocation location)
        : base(location)
    {
        NamespaceName    = namespaceName;
        TypeName         = typeName;
        GenericArguments = genericArguments;
    }

    /// <summary>
    /// The name of the namespace of the type reference.
    /// </summary>
    public NamespaceName? NamespaceName { get; }

    /// <summary>
    /// The name of the type.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// The generic arguments passed to the type.
    /// </summary>
    public IEnumerable<TypeRefNode> GenericArguments { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}