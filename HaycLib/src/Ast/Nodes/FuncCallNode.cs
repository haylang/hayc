using HaycLib.Ast.Interfaces;
using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class FuncCallNode : AstNode, IExpression
{
    public FuncCallNode(
        ObjAccessNode func,
        List<AstNode> args,
        FileLocation sourceLocation)
        : base(sourceLocation)
    {
        Func = func;
        Args = args;
    }

    /// <summary>
    /// The called function.
    /// </summary>
    public ObjAccessNode Func { get; }

    /// <summary>
    /// The passed arguments.
    /// </summary>
    public List<AstNode> Args { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}