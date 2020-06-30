using System;
using System.Diagnostics;

namespace UI.Core.Extensions
{
    /// <summary>
    /// Class that contains useful extension methods for Stopwatch.
    /// </summary>
    public static class StopwatchExtensions
    {
        public static long ElapsedSeconds(this Stopwatch stopwatch)
        {
            return stopwatch == null
                ? throw new ArgumentNullException(nameof(stopwatch))
                : stopwatch.ElapsedTicks / Stopwatch.Frequency;
        }

        /// <summary>
        /// Returns the elapsed time in microseconds.
        /// </summary>
        /// <param name="stopwatch">The stopwatch object.</param>
        /// <returns>The elapsed time in microseconds.</returns>
        public static long ElapsedMicroseconds(this Stopwatch stopwatch)
        {
            return stopwatch == null
                ? throw new ArgumentNullException(nameof(stopwatch))
                : (1000000 * stopwatch.ElapsedTicks) / Stopwatch.Frequency;
        }

        /// <summary>
        /// Prints the elapsed time using the most appropriate unit.
        /// </summary>
        /// <param name="stopwatch">The stopwatch object.</param>
        /// <returns>The string with printed value.</returns>
        public static string Print(this Stopwatch stopwatch)
        {
            if (stopwatch == null)
            {
                throw new ArgumentNullException(nameof(stopwatch));
            }

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                return $"{(double)stopwatch.ElapsedMilliseconds / 1000:N} sec";
            }
            else if (stopwatch.ElapsedMicroseconds() > 1000)
            {
                return $"{(double)stopwatch.ElapsedMicroseconds() / 1000:N} msec";
            }
            else
            {
                return $"{stopwatch.ElapsedMicroseconds():N} μsec";
            }
        }
    }
}