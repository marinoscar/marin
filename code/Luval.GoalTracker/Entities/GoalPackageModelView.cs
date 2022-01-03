using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class GoalPackageModelView
    {
        public GoalPackageModelView()
        {
            DateTime = DateTime.Today;
            Questions = new List<GoalEntryModelView>();
        }
        public DateTime DateTime { get; set; }
        public List<GoalEntryModelView> Questions { get; set; }
    }
}
