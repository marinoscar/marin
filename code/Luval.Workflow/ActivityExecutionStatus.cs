using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Workflow
{
    public class ActivityExecutionStatus : StatusEntity
    {
        public string ActivitySessionId { get; set; }
        public string ActivityType { get; set; }
        public string ActivityName { get; set; }
    }
}
