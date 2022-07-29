using HaycLib.Ast.Data;
using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class ObjAccessNode : AstNode, IExpression
{
    public ObjAccessNode(
        NamespaceName? namespaceName,
        string objectName,
        IEnumerable<ObjAccessNode> genericArguments,
        FileLocation sourceLocation)
        : base(sourceLocation)
    {
        Namespace             = namespaceName;
        ObjectName            = objectName;
        GenericArguments = genericArguments;
    }

    /// <summary>
    /// The namespace of the object.
    /// </summary>
    public NamespaceName? Namespace { get; }

    /// <summary>
    /// The name of the object.
    /// </summary>
    public string ObjectName { get; }

    /// <summary>
    /// The generic arguments passed to the object access.
    /// </summary>
    public IEnumerable<ObjAccessNode> GenericArguments { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}