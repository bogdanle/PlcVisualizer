using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UI.Core.Extensions;

public static class StringExtensions
{
    /// <param name="value">The source string object.</param>
    extension(string value)
    {
        public int? ToInt()
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

        public double? ToDouble()
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

        public decimal? ToDecimal()
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

        public string Capitalize()
        {
            return value.First().ToString().ToUpper() + value.Substring(1);
        }

        public string SplitCamelCase()
        {
            return string.Join(" ", Regex.Split(value, @"(?<!^)(?=[A-Z])"));
        }

        /// <summary>
        /// Extract a substring from a string using start and end indices.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>The extracted substring.</returns>
        public string Extract(int start, int end)
        {
            return value.Substring(start, end - start);
        }

        /// <summary>
        /// Extracts the string between the characters c1 and c2.
        /// </summary>
        /// <param name="c1">First character.</param>
        /// <param name="c2">Second character.</param>
        /// <returns>The extracted string.</returns>
        public string Extract(char c1, char c2)
        {
            int pos = value.IndexOf(c1);
            if (pos >= 0)
            {
                return value.Extract(pos + 1, value.IndexOf(c2, pos + 2));
            }

            return null;
        }

        /// <summary>
        /// Extracts the string between the characters c1 and c2 starting from specific index.
        /// </summary>
        /// <param name="startPos">The start index.</param>
        /// <param name="c1">First character.</param>
        /// <param name="c2">Second character.</param>
        /// <returns>The extracted string.</returns>
        public string Extract(int startPos, char c1, char c2)
        {
            int pos = value.IndexOf(c1, startPos);
            if (pos >= 0)
            {
                return value.Extract(pos + 1, value.IndexOf(c2, pos + 2));
            }

            return null;
        }

        /// <summary>
        /// Breaks the string into an array of single lines.
        /// </summary>
        /// <param name="removeEmptyEntries">True if the empty entries should be removed.</param>
        /// <returns>The output array.</returns>
        public string[] GetLines(bool removeEmptyEntries = true)
        {
            return value?.Split([Environment.NewLine], removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }
    }
}
