using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class RequestResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public long SizeInBits { get; set; }
    }
}
