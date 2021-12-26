using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marin.Web.Workers
{
    public class TimerWorkerOptions
    {
        public DateTimeOffset? StartTime { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
