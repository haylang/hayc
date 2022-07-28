using HaycLib.Location;

namespace HaycLib.Ast.DataObjects;

/// <summary>
/// This is an attribute on a Hay source object, *not* a .NET attribute.
/// </summary>
public record HayAttribute(string Name, FileLocation Location);