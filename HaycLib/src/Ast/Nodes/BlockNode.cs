using System.Collections;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class BlockNode : AstNode
{
    public BlockNode(FileLocation location) : base(location)
    {
        Children = new List<AstNode>();
    }
    
    /// <summary>
    /// The nodes in the block.
    /// </summary>
    public List<AstNode> Children { get; }
    
    public override T Accept<T>(IVisitor<T> visitor)
    {
        foreach (AstNode child in Children)
        {
            child.Accept(visitor);
        }
        return default!;
    }
}