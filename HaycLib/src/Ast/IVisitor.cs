using HaycLib.Ast.Nodes;

namespace HaycLib.Ast;

public interface IVisitor<TReturn>
{
    public TReturn Visit(AstNode node)
    {
        return node.Accept(this);
    }
    
    public TReturn FileNode(FileNode fileNode);
}