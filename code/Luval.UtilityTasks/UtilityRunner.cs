using log4net;
using Luval.Data;
using Luval.UtilityTasks.SpeedTasks;
using Luval.UtilityTasks.TimeSeriesTasks;
using Luval.Workflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Luval.UtilityTasks
{
    public class UtilityRunner : Runner
    {
        public UtilityRunner():base(BuildActivities(), BuildStore(), BuildLogger())
        {

        }
        private static IEnumerable<IActivity> BuildActivities()
        {
            var localConnString = ConfigurationManager.AppSettings["Db.Job.Local"];
            var azureConnString = ConfigurationManager.AppSettings["Db.Job.Azure"];
            
            

            return new List<IActivity>(new IActivity[] { 
                new SpeedTestActivity(),
                new SpeedTestConverToTimeSeries(),
                new AddTimeSeriesActivity() { 
                    TimeSeriesMemoryLocation = "speedtest.timeseries",
                    ConnectionString = localConnString
                },
                new AddTimeSeriesActivity() {
                    TimeSeriesMemoryLocation = "speedtest.timeseries",
                    ConnectionString = azureConnString
                }
            });
        }

        private static ISessionStore BuildStore()
        {
            return new DbSessionStore("Server=.;Database=MarinDb;Trusted_Connection=True;");
        }

        private static ILogger BuildLogger()
        {
            return new Log4NetProvider().CreateLogger();
        }

    }
}
