using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace UI.Core.Extensions;

/// <summary>
/// Extension class that contains several useful methods for enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description attribute for an enum value.
    /// </summary>
    /// <param name="value">The enumeration item we went to get the description for.</param>
    /// <returns>Returns the description string associated with the item or null if the description attribute was not found.</returns>
    public static string GetDescription(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        object[] attrs = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attrs.Any())
        {
            return (attrs.First() as DescriptionAttribute).Description;
        }

        return value.ToString();
    }

    /// <summary>
    /// Converts string to matching enum value.
    /// </summary>
    /// <typeparam name="T">The underlying enumeration type.</typeparam>
    /// <param name="description">The enum value as a string.</param>
    /// <param name="defaultValue">Default value used if supplied description string is empty.</param>
    /// <returns>The enum value matching the description string or the default value if string supplied was blank.</returns>
    public static T FromString<T>(string description, T defaultValue = default(T))
    {
        if (!string.IsNullOrEmpty(description))
        {
            return (T)Enum.Parse(typeof(T), description);
        }

        return defaultValue;
    }

    /// <summary>
    /// Generates a collection of items that are included in given enumeration type.
    /// </summary>
    /// <typeparam name="T">The underlying enumeration type.</typeparam>
    /// <returns>The collection of enum items.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is necessary in this case.")]
    public static IEnumerable Generate<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<object>().Select(e => new { Value = (T)e, Description = ((Enum)e).GetDescription() });
    }
}