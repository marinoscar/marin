using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Luval.Common
{
    public static class CodeGenerator
    {
        public static DateTime root = new DateTime(1983, 1, 19);
        public static string GetCode(Func<decimal> getVal)
        {
            return NumberEncoder.ToBase36(getVal());
        }

        public static string GetCode()
        {
            Thread.Sleep(1);
            var dec = (decimal)DateTime.UtcNow.Subtract(root).TotalMilliseconds;
            var firstDigit = (new Random()).Next(0, 36);
            return string.Concat(GetCode(() => { return (decimal)firstDigit; }), GetCode(() => { return dec; }));
        }
    }
}
