using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class PackageResult
    {
        public Target Target { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal SizeInBits { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public decimal BitsPerSecond { get; set; }
    }
}
