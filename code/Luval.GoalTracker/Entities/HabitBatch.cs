using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitBatch
    {
        public HabitBatch()
        {
            BatchId = CodeGenerator.GetCode();
            Habits = new List<HabitDefinition>();
        }
        public string BatchId { get; set; }
        public List<HabitDefinition> Habits { get; set; }
    }
}
