using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalBatch
    {
        public GoalBatch()
        {
            BatchId = CodeGenerator.GetCode();
            Goals = new List<GoalDefinition>();
        }
        public string BatchId { get; set; }
        public List<GoalDefinition> Goals { get; set; }
    }
}
