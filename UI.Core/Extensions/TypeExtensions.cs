using System;

namespace UI.Core.Extensions
{
    public static class TypeExtensions
    {
        public static string GetTypeString(this Type type)
        {
            string typeString = type.ToString();
            int pos = typeString.LastIndexOf('+');
            if (pos == -1)
            {
                pos = typeString.LastIndexOf('.');
            }

            typeString = typeString.Substring(pos + 1);

            if (typeString.LastIndexOf("[]") == typeString.Length - 2)
            {
                return typeString;
            }

            return typeString.Replace("]", string.Empty);
        }

        public static bool IsCollectionType(this Type type)
        {
            return type.GetInterface("ICollection`1") != null;
        }

        public static bool IsComplexType(this Type type)
        {
            return type.IsClass && type != typeof(string);
        }
    }
}