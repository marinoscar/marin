using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class TestOptions
    {
        public TestOptions(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");
            Token = token;
            UseHttps = true;
            UrlCount = 5;
            Unit = TestUnit.Mb;
            TimeoutInMilliseconds = (5 * 60 * 1000);
            ConcurrentConnections = UrlCount;
        }

        public string Token { get; private set; }
        public bool UseHttps { get; set; }
        public short UrlCount { get; set; }
        public TestUnit  Unit { get; set; }
        public int TimeoutInMilliseconds { get; set; }
        public short ConcurrentConnections { get; set; }

    }
}
