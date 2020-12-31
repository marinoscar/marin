using Luval.Data.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Luval.Workflow
{
    [TableName("ActivitySession")]
    public class SessionContext : StatusEntity
    {

        public SessionContext()
        {
            UserName = Environment.UserName;
            MachineName = Environment.MachineName;
            Data = new ConcurrentDictionary<string, object>();
            ActivityExecutions = new List<ActivityExecutionStatus>();
        }

        public string RunnerType { get; internal set; }
        public string MachineName { get; }
        public string UserName { get; }
        [NotMapped]
        public IDictionary<string, object> Data { get; private set; }
        [NotMapped]
        public IList<ActivityExecutionStatus> ActivityExecutions { get; }
        [NotMapped]
        public ILogger Logger { get; internal set; }
    }
}
