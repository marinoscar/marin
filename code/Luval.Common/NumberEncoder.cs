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
        public static string ToBase36(long value)
        {
            var result = "";
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            while (value > 0)
            {
                result = chars[(int)(value % 36)] + result;
                value /= 36;
            }
            return result;
        }
    }
}
