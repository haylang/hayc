using HaycLib.Ast.Nodes;

namespace HaycLib.Ast;

public interface IVisitor<T>
{
    public T Visit(AstNode node)
    {
        return node.Accept(this);
    }
    
    public T FileNode(FileNode fileNode);
}