namespace HaycLib.Ast;

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
}