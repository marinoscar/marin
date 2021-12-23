using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Luval.Common
{
    public static class CodeGenerator
    {
        
        public static string GetCode(Func<long> getVal)
        {
            return NumberEncoder.ToBase36(getVal());
        }

        public static string GetCode()
        {
            var numGen = new NumberGenerator();
            string code;
            lock (numGen)
            {
                code = string.Concat(GetCode(() => { return (long)numGen.GetRandomNum(); }), GetCode(() => { return numGen.GetTimeNumber(); }));
            }
            return code;
        }
    }

    internal class NumberGenerator
    {
        public static DateTime root = new DateTime(1983, 1, 19);

        public long GetRandomNum()
        {
            Thread.Sleep(1);
            return (new Random()).Next(1, 36);
        }

        public long GetTimeNumber()
        {
            Thread.Sleep(1);
            return Convert.ToInt64(DateTime.UtcNow.Subtract(root).TotalMilliseconds);
        }
    }

}
