using HaycLib.Location;

namespace HaycLib.Ast;

public abstract class AstNode
{
    protected AstNode(FileLocation sourceLocation)
    {
        SourceLocation = sourceLocation;
    }

    /// <summary>
    /// The location of this node in the source code.
    /// </summary>
    public FileLocation SourceLocation { get; }
    
    /// <summary>
    /// Invokes the expected method on the visitor.
    /// </summary>
    public abstract T Accept<T>(IVisitor<T> visitor);
}