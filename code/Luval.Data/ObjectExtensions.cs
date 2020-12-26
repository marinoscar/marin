using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Tranfors the <see cref="object"/> value to a sql formatted string
        /// </summary>
        /// <param name="value">The value to transformed</param>
        /// <returns>The formatted sql string</returns>
        public static string ToSql(this object value)
        {
            return SqlFormatter.Format(null, value);
        }

        /// <summary>
        /// Validates if the <see cref="object"/> value is null or <see cref="DBNull"/>
        /// </summary>
        /// <param name="o">The value to validate</param>
        /// <returns>true if the <see cref="object"/> value is null or <see cref="DBNull"/> otherwise false</returns>
        public static bool IsNullOrDbNull(this object o)
        {
            return null == o || Convert.IsDBNull(o);
        }
    }
}
