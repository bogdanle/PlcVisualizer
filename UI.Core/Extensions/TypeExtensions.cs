using System;

namespace UI.Core.Extensions;

public static class TypeExtensions
{
    extension(Type type)
    {
        public string GetTypeString()
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

        public bool IsCollectionType()
        {
            return type.GetInterface("ICollection`1") != null;
        }

        public bool IsComplexType()
        {
            return type.IsClass && type != typeof(string);
        }
    }
}