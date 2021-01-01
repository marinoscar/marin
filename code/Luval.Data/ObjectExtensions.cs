using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Checks if the object type is a primity type, including types like date and time structs
        /// </summary>
        /// <param name="o">The object type to evaluate</param>
        /// <returns>A bool value</returns>
        public static bool IsPrimitiveType(this object o)
        {
            if (IsNullOrDbNull(o)) return true;
            if (o.GetType().IsPrimitive || o.GetType().IsValueType || o.GetType().IsEnum) return true;
            return new[] { typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(DateTimeOffset) }.Any(i => i.IsAssignableFrom(o.GetType()));
        }
    }
}
