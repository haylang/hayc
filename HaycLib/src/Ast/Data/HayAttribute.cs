using HaycLib.Location;

namespace HaycLib.Ast.Data;

/// <summary>
/// This is an attribute on a Hay source object, *not* a .NET attribute.
/// </summary>
public record HayAttribute(string Name, FileLocation Location);