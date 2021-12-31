using Luval.Common;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalEntry : BaseAuditEntity
    {
        public string GoalDefinitionId { get; set; }
        [TableReference]
        public GoalDefinition GoalDefinition { get; set; }
        public DateTime GoalDateTime { get; set; }
        public double NumericValue { get; set; }
        public string StringValue { get; set; }
    }
}
