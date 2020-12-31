using Luval.Workflow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UtilityTasks
{
    public abstract class ActivityTask : IActivity
    {
        private readonly string _name;
        private readonly string _code;

        public ActivityTask(string name, string id)
        {
            _name = name;
            _code = id;
        }

        public IActivityName Name => new ActivityName() { DisplayName = _name, Id = _code, ActivityType = GetType() };

        public Task ExecuteAsync(SessionContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() => DoActivity(context), cancellationToken);
        }

        protected abstract void DoActivity(SessionContext context);
    }
}
