using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luval.Web.Common.Workers
{
    public class TimerWorkerOptions
    {
        public DateTimeOffset? StartTime { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
