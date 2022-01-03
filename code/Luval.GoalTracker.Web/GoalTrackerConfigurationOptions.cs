using Luval.Web.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Web
{
    public class GoalTrackerConfigurationOptions : LuvalConfigurationOptions
    {
        public GoalTrackerConfigurationOptions(IWebHostEnvironment environment) : base(environment)
        {

        }
    }
}
