using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.FastSpeedTestApi
{
    public class Target
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }

        public Uri Uri { get { return new Uri(Url); } }

        public string UrlHost { get { return Uri.Host; } }
    }
}
