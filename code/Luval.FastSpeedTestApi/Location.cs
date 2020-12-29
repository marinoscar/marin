using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class Location
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
    }
}
