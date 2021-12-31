using Luval.Common;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalTarget : BaseAuditEntity
    {
        public string GoalDefinitionId { get; set; }
        [TableReference]
        public GoalDefinition GoalDefinition { get; set; }
        public DateTime TargetDate { get; set; }
        public double OptimalValue { get; set; }
        public double LowerValue { get; set; }
        public double HigherValue { get; set; }
    }
}
