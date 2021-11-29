using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Schema;

namespace Luval.Common
{
    public static class NumberEncoder
    {

        private static char[] base16 = Get16();
        private static char[] base36 = Get36();

        public static string ToBase(decimal value, char[] baseToUse)
        {
            var divisor = baseToUse.Length - 1;
            var vals = new List<string>();
            var q = Math.Round((value / divisor), 0);
            int r = Convert.ToInt32(value % divisor);
            while (q > 0)
            {
                vals.Add(baseToUse[r].ToString());
                q = Math.Round((q / divisor), 0);
                r = Convert.ToInt32(q % divisor);
            }
            vals.Reverse();
            return string.Join("", vals);
        }

        public static string ToHex(decimal value)
        {
            return ToBase(value, base16);
        }

        public static string ToBase36(decimal value)
        {
            return ToBase(value, base36);
        }

        private static char[] Get16()
        {
            return new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        }

        private static char[] Get36()
        {
            var values = new List<char>();
            for (int i = 48; i < 58; i++)
            {
                values.Add((char)i);
            }
            for (int i = 65; i < 91; i++)
            {
                values.Add((char)i);
            }
            return values.ToArray();
        }


    }
}
