using System.Collections;
using HaycLib.Ast.DataObjects;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

/// <summary>
/// Represents a file.
/// </summary>
public class FileNode : AstNode
{
    public FileNode(NamespaceName namespaceName, IEnumerable<NamespaceName> imports, BlockNode body)
        : base(namespaceName.Location)
    {
        Namespace = namespaceName;
        Imports   = imports;
        Body = body;
    }

    /// <summary>
    /// The namespace of the file.
    /// </summary>
    public NamespaceName Namespace { get; }

    /// <summary>
    /// The imported namespaces within the file.
    /// </summary>
    public IEnumerable<NamespaceName> Imports { get; }

    /// <summary>
    /// The file body.
    /// </summary>
    public BlockNode Body { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.FileNode(this);
    }
}