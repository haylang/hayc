namespace HaycLib.Ast;

/// <summary>
/// Any node in the AST.
/// </summary>
public abstract class AstNode
{
    protected AstNode()
    {
        Children = new List<AstNode>();
    }
    
    /// <summary>
    /// The children of this node.
    /// </summary>
    public List<AstNode> Children { get; }

    /// <summary>
    /// Invokes the expected method on the visitor.
    /// </summary>
    public abstract T Accept<T>(IVisitor<T> visitor);
}