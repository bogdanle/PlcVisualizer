using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UI.Core.Extensions
{
    public static class CalendarHelper
    {
        /// <summary>
        /// Returns first day of the given week accordingly to ISO8601.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="weekOfYear">The week number.</param>
        /// <returns>DateTime object which is the first day of the given week in the year specified.</returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        /// <summary>
        /// Returns first day of the week for the date specified accordingly to ISO8601.
        /// </summary>        
        /// <param name="dateTime">The DateTime object.</param>
        /// <returns>DateTime object which is the first day of the week for the date specified.</returns>
        public static DateTime FirstDateOfWeek(DateTime dateTime)
        {
            return FirstDateOfWeek(dateTime.Year, GetWeekNumberFromDate(dateTime));
        }

        /// <summary>
        /// Returns the week of the year for the date specified.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>The week number.</returns>
        public static int GetWeekNumberFromDate(DateTime date)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        /// <summary>
        /// Get Date Range from start date.
        /// </summary>
        /// <param name="startDate">Start Date.</param>
        /// <param name="endDate">End Date.</param>
        /// <returns>Returns List of date time scope for Dev Express Date Navigator control.</returns>
        public static IEnumerable<DateTime> GetDateRange(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }
    }
}
