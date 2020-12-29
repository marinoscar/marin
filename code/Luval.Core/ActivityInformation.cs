using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Core
{
    public class ActivityInformation
    {
        public ActivityInformation(DateTimeOffset start)
        {
            StartTime = start;
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;
            EndTime = null;
        }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string MachineName { get; }
        public string UserName { get; }
    }
}
