using HaycLib.Location;

namespace HaycLib.Ast.Nodes;

public class VariableAssignmentNode : AstNode
{
    public VariableAssignmentNode(ObjAccessNode variable, AstNode value, FileLocation location)
        : base(location)
    {
        Variable = variable;
        Value    = value;
    }
    
    /// <summary>
    /// The variable to assign to.
    /// </summary>
    public ObjAccessNode Variable { get; }
    
    /// <summary>
    /// The value to assign to the variable.
    /// </summary>
    public AstNode Value { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}