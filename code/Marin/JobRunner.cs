using log4net.Core;
using Luval.Workflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marin
{
    public static class JobRunner
    {
        /// <summary>
        /// Runs all of the jobs by creating the instances from the runner string, runners are splitted by ;
        /// internally it needs to be the full assmembly name and then the type the following way
        /// {assembly1_name},{version},{culture},{token}|{className1};{assembly2_name},{version},{culture},{token}|{className2}
        /// </summary>
        /// <param name="runnerString">Runner types to build</param>
        public static void DoRun(string runnerString, Microsoft.Extensions.Logging.ILogger logger)
        {
            var runners = new List<Runner>();
            if (string.IsNullOrWhiteSpace(runnerString)) return;
            var types = runnerString.Split(';');
            foreach (var type in types)
            {
                var runner = LoadRunner(type);
                if (runner == null) logger.LogWarning("Unable to load {0}", type);
                else runners.Add(runner);
            }
            foreach (var runner in runners)
            {
                logger.LogInformation("Running {0}", runner.GetType().Name);
                var task = runner.StartSession();
                task.Wait();
            }
        }

        private static Runner LoadRunner(string typeString)
        {
            var name = typeString.Split('|');
            var jobAssembly = Assembly.Load(name[0]);
            var type = jobAssembly.GetType(name[1]);
            if (!typeof(Runner).IsAssignableFrom(type)) return null;
            return (Runner)jobAssembly.CreateInstance(type.FullName);
        }
    }
}
