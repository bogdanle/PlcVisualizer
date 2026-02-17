using System.Linq;

namespace UI.Core.Extensions;

/// <summary>
/// Class that contains extensions for object.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Returns the class name of the object (no namespaces).
    /// </summary>
    /// <param name="value">The object.</param>
    /// <returns>The class name of the source object.</returns>
    public static string GetClassName(this object value)
    {
        return value?.GetType().ToString().Split('.').Last();
    }
}