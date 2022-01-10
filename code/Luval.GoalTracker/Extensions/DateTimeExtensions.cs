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
            return DateTime.Today.Date.AddHours(-6);
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
    }
}
