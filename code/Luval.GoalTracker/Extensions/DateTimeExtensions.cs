using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Extensions
{
    /// <summary>
    /// Set of extensions for the <see cref="DateTime"/> structure in the app
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the value for <seealso cref="DateTime.Today"/> for the provided timezone
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> instance</param>
        /// <param name="tz">The timezone</param>
        /// <returns>A <see cref="DateTime"/> value</returns>
        public static DateTime AppDateTime(this DateTime dt, string tz)
        {
            return DateTime.UtcNow.AddHours(-6);
        }

        /// <summary>
        /// Gets the value for <seealso cref="DateTime.Today"/> for the CST timezone
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> instance</param>
        /// <returns>A <see cref="DateTime"/> value</returns>
        public static DateTime AppDateTime(this DateTime dt)
        {
            return AppDateTime(dt, "Central Standard Time");
        }

        /// <summary>
        /// Gets the date for the start of the week
        /// </summary>
        public static DateTime WeekStart(this DateTime dt)
        {
            var today = dt.AppDateTime().Date;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return today.AddDays(-6);
                case DayOfWeek.Monday:
                    return today;
                case DayOfWeek.Tuesday:
                    return today.AddDays(-1);
                case DayOfWeek.Wednesday:
                    return today.AddDays(-2);
                case DayOfWeek.Thursday:
                    return today.AddDays(-3);
                case DayOfWeek.Friday:
                    return today.AddDays(-4);
                case DayOfWeek.Saturday:
                    return today.AddDays(-5);
                default:
                    return today;
            }
        }

        public static DateTime MonthStart(this DateTime dt)
        {
            var today = dt.AppDateTime().Date;
            return new DateTime(today.Year, today.Month, 1).Date;
        }

        public static DateTime YearStart(this DateTime dt)
        {
            var today = dt.AppDateTime().Date;
            return new DateTime(today.Year, 1, 1).Date;
        }
    }
}
