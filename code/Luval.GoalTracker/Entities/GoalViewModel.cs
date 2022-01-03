using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    [TableName("VW_GoalTrackerList")]
    public class GoalViewModel
    {
        public string GoalId { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string ReminderDaysOfWeek { get; set; }
        public string LastEntry { get; set; }
        public string EntryCount { get; set; }

    }
}
