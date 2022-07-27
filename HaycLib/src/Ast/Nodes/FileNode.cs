namespace HaycLib.Ast.Nodes;

/// <summary>
/// Represents a file.
/// </summary>
public class FileNode : AstNode
{
    public FileNode(string namespaceName, string[] imports)
    {
        Namespace = namespaceName;
        Imports   = imports;
    }
    
    /// <summary>
    /// The namespace of the file.
    /// </summary>
    public string Namespace { get; }
    
    /// <summary>
    /// The imported namespaces within the file.
    /// </summary>
    public string[] Imports { get; }

    /// <inheritdoc cref="AstNode.Accept{T}"/>
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.FileNode(this);
    }
}