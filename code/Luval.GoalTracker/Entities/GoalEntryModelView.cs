using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalEntryModelView
    {
        public string DefinitionId { get; set; }
        public string Question { get; set; }
        public double? NumberValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Type { get; set; }
    }
}
