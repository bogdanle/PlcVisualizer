using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UI.Core.Extensions
{
    public static class StringExtensions
    {
        public static int? ToInt(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (int.TryParse(value, out int d))
                {
                    return d;
                }
            }

            return null;
        }

        public static double? ToDouble(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (double.TryParse(value, out double d))
                {
                    return d;
                }
            }

            return null;
        }

        public static decimal? ToDecimal(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (decimal.TryParse(value, out decimal d))
                {
                    return d;
                }
            }

            return null;
        }

        public static string Capitalize(this string value)
        {
            return value.First().ToString().ToUpper() + value.Substring(1);
        }

        public static string SplitCamelCase(this string value)
        {
            return string.Join(" ", Regex.Split(value, @"(?<!^)(?=[A-Z])"));
        }

        /// <summary>
        /// Extract a substring from a string using start and end indices.
        /// </summary>
        /// <param name="text">The source string object.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>The extracted substring.</returns>
        public static string Extract(this string text, int start, int end)
        {
            return text.Substring(start, end - start);
        }

        /// <summary>
        /// Extracts the string between the characters c1 and c2.
        /// </summary>
        /// <param name="text">The source string object.</param>
        /// <param name="c1">First character.</param>
        /// <param name="c2">Second character.</param>
        /// <returns>The extracted string.</returns>
        public static string Extract(this string text, char c1, char c2)
        {
            int pos = text.IndexOf(c1);
            if (pos >= 0)
            {
                return text.Extract(pos + 1, text.IndexOf(c2, pos + 2));
            }

            return null;
        }

        /// <summary>
        /// Extracts the string between the characters c1 and c2 starting from specific index.
        /// </summary>
        /// <param name="text">The source string object.</param>
        /// <param name="startPos">The start index.</param>
        /// <param name="c1">First character.</param>
        /// <param name="c2">Second character.</param>
        /// <returns>The extracted string.</returns>
        public static string Extract(this string text, int startPos, char c1, char c2)
        {
            int pos = text.IndexOf(c1, startPos);
            if (pos >= 0)
            {
                return text.Extract(pos + 1, text.IndexOf(c2, pos + 2));
            }

            return null;
        }

        /// <summary>
        /// Breaks the string into an array of single lines.
        /// </summary>
        /// <param name="text">the string object.</param>
        /// <param name="removeEmptyEntries">True if the empty entries should be removed.</param>
        /// <returns>The output array.</returns>
        public static string[] GetLines(this string text, bool removeEmptyEntries = true)
        {
            return text?.Split(new[] { Environment.NewLine }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None) ?? throw new ArgumentNullException(nameof(text));
        }
    }
}