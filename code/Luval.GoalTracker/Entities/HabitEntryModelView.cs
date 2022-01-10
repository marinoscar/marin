using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitEntryModelView
    {
        public string DefinitionId { get; set; }
        public string Question { get; set; }
        public double? NumberValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Notes { get; set; }
        public int? Difficulty { get; set; }
        public string Type { get; set; }
        public DateTime EntryDateTime { get; set; }
    }
}
