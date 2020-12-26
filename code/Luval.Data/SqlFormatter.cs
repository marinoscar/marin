using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Luval.Data
{
    public class SqlFormatter : IFormatProvider, ICustomFormatter
    {
        public readonly static SqlFormatter Instance = new SqlFormatter();

        public object GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return Format(format, arg);
        }

        public static readonly string[] StringComparisonOperators = new[] { "equals", "startsWith", "endsWith", "contains" };

        public static string Format(string format, object o)
        {
            var prefix = format == "equals" ? "= " : string.Empty;

            if (o.IsNullOrDbNull())
            {
                if (StringComparisonOperators.Contains(format))
                {
                    return "IS NULL";
                }

                return "NULL";
            }

            if (o is DateTime)
            {
                return prefix + string.Format("'{0:yyyy-MM-dd HH:mm:ss.fff}'", o);
            }

            if (o is string)
            {
                var s = (string)o;

                if (format == "verbatim")
                {
                    return s;
                }

                if (format == "name")
                {
                    return "[{0}]".Fi(s.Replace("]", "]]"));
                }

                s = s.Replace("'", "''");
                if (format == "startsWith")
                {
                    s = s.EscapeMagicSqlLikeChars();
                    return "LIKE '{0}%'".Fi(s);
                }

                if (format == "endsWith")
                {
                    s = s.EscapeMagicSqlLikeChars();
                    return "LIKE '%{0}'".Fi(s);
                }

                if (format == "contains")
                {
                    s = s.EscapeMagicSqlLikeChars();
                    return "LIKE '%{0}%'".Fi(s);
                }

                return prefix + "'{0}'".Fi(s);
            }

            if (o is bool)
            {
                return prefix + ((bool)o ? "1" : "0");
            }

            if (o is ICollection<byte>)
            {
                var bytes = (o as ICollection<byte>);
                var builder = new StringBuilder(prefix + "0x", bytes.Count * 2 + 8);

                foreach (var b in bytes)
                {
                    builder.Append(b.ToHex());
                }

                return builder.ToString();
            }

            if (o is IEnumerable)
            {
                var builder = new StringBuilder(prefix, 32);
                builder.Append("(");

                foreach (var item in (IEnumerable)o)
                {
                    builder.AppendFormat("{0},", Format(null, item));
                }

                if (1 == builder.Length)
                {
                    builder.Append("NULL");
                }
                else
                {
                    builder.Length -= 1;
                }

                builder.Append(")");
                return builder.ToString();
            }

            if (o is int)
            {
                return prefix + ((int)o).ToString(CultureInfo.InvariantCulture);
            }

            if (o is double)
            {
                return prefix + ((double)o).ToString(CultureInfo.InvariantCulture);
            }

            return prefix + "{0}".Fi(o);
        }
    }
}
