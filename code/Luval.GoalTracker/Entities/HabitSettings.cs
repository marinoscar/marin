using Luval.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitSettings : BaseEntity
    {
        public HabitSettings()
        {
            WeeklyReminders = DayOfWeek.Saturday;
            MonthlyReminders = DayOfWeek.Sunday;
            DailyReminderHour = 8;
            WeeklyReminderHour = 10;
            MonthlyReminderHour = 11;
        }


        public DayOfWeek WeeklyReminders { get; set; }
        public DayOfWeek MonthlyReminders { get; set; }
        public int DailyReminderHour { get; set; }
        public int WeeklyReminderHour { get; set; }
        public int MonthlyReminderHour { get; set; }
    }
}
