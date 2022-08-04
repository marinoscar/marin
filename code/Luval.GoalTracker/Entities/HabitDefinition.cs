﻿using Luval.Common;
using Luval.DataStore.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitDefinition : BaseEntity, IValidate
    {
        public HabitDefinition() : base()
        {
            Entries = new List<HabitEntry>();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public string UnitOfMeasure { get; set; }
        public double? DailyTarget { get; set; }
        public double? WeeklyTarget { get; set; }
        public double? MonthlyTarget { get; set; }
        public double? YearlyTarget { get; set; }
        public double? WeeklyProgress { get; set; }
        public double? MonthlyProgress { get; set; }
        public double? YearlyProgress { get; set; }
        public string Frequency { get; set; }
        public DateTime? Reminder { get; set; }
        public string ReminderDaysOfWeek { get; set; }
        public string Notes { get; set; }
        public bool IsInactive { get; set; }
        public int Sort { get; set; }
        public DateTime? CurrentWeek { get; set; }

        [NotMapped]
        public List<HabitEntry> Entries { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Id) &&
                   !string.IsNullOrWhiteSpace(Question);
        }
    }
}