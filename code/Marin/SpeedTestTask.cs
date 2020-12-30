using Luval.FastSpeedTestApi;
using Luval.Workflow;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marin
{
    public class SpeedTestTask : IActivity
    {
        public IActivityName Name { get { return new ActivityName() { ActivityType = GetType(), Id = "MARIN-001", DisplayName = "Speed Test" }; } }

        public Task ExecuteAsync(SessionContext context, CancellationToken cancellationToken)
        {
            var test = new SpeedTest("YXNkZmFzZGxmbnNkYWZoYXNkZmhrYWxm", context.Logger);
            return Task.Run(() => test.GetSpeed());
        }
    }

    public class SpeedTestRunner : Runner
    {
        public SpeedTestRunner() : 
            base(new[] { new SpeedTestTask() }, 
                 new DbSessionStore(ConfigurationManager.ConnectionStrings["UserProfile"].ConnectionString),
                 new ConsoleLogger())
        {

        }
    }
}
