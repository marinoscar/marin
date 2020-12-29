using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class ApiTargetResult
    {
        public Client Client { get; set; }
        public List<Target> Targets {get;set;}

        public IRestResponse Response { get; set; }
    }
}
