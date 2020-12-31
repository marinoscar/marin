using Luval.FastSpeedTestApi;
using Luval.UtilityTasks.Models;
using Luval.Workflow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.UtilityTasks.SpeedTasks
{
    public class SpeedTestConverToTimeSeries : ActivityTask
    {
        public SpeedTestConverToTimeSeries() : base("Covert Speed Test To Time Series", "MARIN-003")
        {

        }
        protected override void DoActivity(SessionContext context)
        {
            var speedTestResult = context.GetData<SpeedTestResult>("speedtest.data");
            if (speedTestResult == default(SpeedTestResult)) throw new ArgumentNullException(nameof(speedTestResult));
            var ts = new TimeSeries("Internet Speed Test", speedTestResult.DownloadSpeed, speedTestResult.TestUnit.ToString(), DateTime.UtcNow);
            context.Data["speedtest.timeseries"] = new[] { ts };
        }
    }
}
