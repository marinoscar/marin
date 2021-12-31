using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalDefinition : BaseAuditEntity
    {
        public string Name { get; set; }
        public GoalFrequency Frequency { get; set; }
    }
}
