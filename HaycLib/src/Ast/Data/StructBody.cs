using System.Collections;
using HaycLib.Ast.Nodes;

namespace HaycLib.Ast.Data;

/// <summary>
/// The body of a struct.
/// </summary>
public record StructBody(
    IEnumerable<StructInitializerNode> Initializers,
    IEnumerable<VariableNode> Fields,
    StructDestructorNode? Destructor);
