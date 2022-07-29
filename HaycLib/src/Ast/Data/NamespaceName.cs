using HaycLib.Location;

namespace HaycLib.Ast.Data;

/// <summary>
/// Contains data for a namespace namespace.
/// </summary>
public record NamespaceName(string Name, FileLocation Location);