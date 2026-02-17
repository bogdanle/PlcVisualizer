using System.Text;

namespace UI.Core.Extensions;

/// <summary>
/// Class that contains various extension methods for StringBuilder.
/// </summary>
public static class StringBuilderExtensions
{
    /// <param name="builder">The StringBuilder object.</param>
    extension(StringBuilder builder)
    {
        /// <summary>
        /// Fills the beginning of the line with spaces to maintain the desired indentation.
        /// </summary>
        /// <param name="indentLevel">Indentation level (multiplies of 4).</param>
        public void Indent(int indentLevel = 1)
        {
            builder?.Append(' ', indentLevel * 4);
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance indenting every line by specified value.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <param name="indentLevel">Indentation level (multiples of 4).</param>
        public void Append(string text, int indentLevel)
        {
            foreach (string line in text.GetLines(false))
            {
                builder?.Append(' ', indentLevel * 4);
                builder?.AppendLine(line);
            }
        }
    }
}