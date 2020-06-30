using System;
using System.Reflection;
using UI.Core.Extensions;

namespace UI.Core
{
    /// <summary>
    /// Simple class that allows to read/write properties dynamically using reflection.
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Gets the value of property with given name from specified object.
        /// </summary>
        /// <param name="obj">The object we read the property from.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The value of property as a string or null if the property was not found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "obj is appropriate name here")]
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
            {
                return null;
            }

            var parts = propertyName.Split(new[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                return GetPropertyValue(GetPropertyValue(obj, parts[0]), parts[1]);
            }

            // Special case for arrays
            int pos = parts[0].IndexOf("[", StringComparison.Ordinal);
            if (pos != -1)
            {
                string propName = parts[0].Substring(0, pos);
                var tmp = obj.GetType().GetProperty(propName)?.GetValue(obj, null);

                if (tmp != null)
                {
                    dynamic arr = tmp;
                    return arr[Convert.ToInt32(parts[0].Extract('[', ']'))];
                }

                return null;
            }

            return obj.GetType().GetProperty(parts[0])?.GetValue(obj, null);
        }

        /// <summary>
        /// Gets the value of property with given name from specified object.
        /// </summary>
        /// <param name="obj">The object we read the property from.</param>
        /// <param name="propertyName">The property name.</param>
        /// <typeparam name="T">The type of property. The retrieved value will be converted to object of that type.</typeparam>
        /// <returns>The value of property. If the property was not found or if its type differs from the type specified we can get either null or exception will be thrown.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "obj is appropriate name here")]
        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var pi = obj.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                return (T)pi.GetValue(obj, null);
            }

            return default(T);
        }

        /// <summary>
        /// Checks if the property of given name exists on the object.
        /// </summary>
        /// <param name="target">The source object.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the property exists, false otherwise.</returns>
        public static bool PropertyExists(object target, string propertyName)
        {
            return GetProperty(target, propertyName) != null;
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return target.GetType().GetProperty(propertyName);
        }

        /// <summary>
        /// Sets the value of property with given name on specified object.
        /// </summary>
        /// <param name="obj">The object we read the property from.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The property's value.</param>  
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "obj is appropriate name here")]
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                return;
            }

            var pi = obj.GetType().GetProperty(propertyName);
            if (pi != null && pi.CanWrite)
            {
                pi.SetValue(obj, value, null);
            }
        }        
    }
}
