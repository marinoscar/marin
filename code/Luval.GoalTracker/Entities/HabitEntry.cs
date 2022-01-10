using Luval.Common;
using Luval.DataStore.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitEntry : BaseEntity
    {
        public string HabitDefinitionId { get; set; }
        [TableReference]
        public HabitDefinition HabitDefinition { get; set; }
        public DateTime EntryDateTime { get; set; }
        public int? Difficulty { get; set; }
        public string Notes { get; set; }
        public double? NumericValue { get; set; }
    }
}
