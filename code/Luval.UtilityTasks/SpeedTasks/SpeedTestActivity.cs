using Luval.Workflow;
using Luval.FastSpeedTestApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UtilityTasks.SpeedTasks
{
    public class SpeedTestActivity : ActivityTask
    {
        public SpeedTestActivity() : base("Speed Test", "MARIN-001")
        {

        }

        protected override void DoActivity(SessionContext context)
        {
            var test = new SpeedTest(context.GetData<string>("speedtest.token", "YXNkZmFzZGxmbnNkYWZoYXNkZmhrYWxm"), context.Logger);
            var result = test.GetSpeed();
            context.Data["speedtest.data"] = result;
        }
    }
}
