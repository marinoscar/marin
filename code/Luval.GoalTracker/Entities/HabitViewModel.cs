using Luval.DataStore.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    [TableName("VW_HabitTrackerList")]
    public class HabitViewModel
    {
        [PrimaryKey]
        public string GoalId { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string Type { get; set; }
        public string ReminderDaysOfWeek { get; set; }
        public DateTime? LastEntry { get; set; }
        public DateTime? FirstEntry { get; set; }
        public long? EntryCount { get; set; }
        public string CreatedByUserId { get; set; }
        public double? WeeklyProgress { get; set; }
        public double? MonthlyProgress { get; set; }
        public double? YearlyProgress { get; set; }
        public int Sort { get; set; }
    }
}
