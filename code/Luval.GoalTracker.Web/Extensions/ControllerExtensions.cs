using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static DateTime GetServerDateTime(this Controller controller)
        {
            return DateTime.Today.AddHours(-6).Date;
        }
    }
}
