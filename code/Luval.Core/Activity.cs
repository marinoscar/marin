using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Core
{
    public abstract class Activity : IActivity
    {
        public Activity(string displayName, string id, ILogger logger)
        {
            Name = new ActivityName() { DisplayName = displayName, Id = id, ActivityType = GetType() };
            Arguments = new Dictionary<string, object>();
            Logger = logger;
        }

        public IActivityName Name { get; private set; }
        protected ILogger Logger { get; private set; }
        protected virtual IDictionary<string, object> Arguments { get; private set; }

        public void AddInputArgument(string name, object value)
        {
            Arguments[name] = value;
        }

        public abstract Task<IActivityResult> Execute();
    }
}
