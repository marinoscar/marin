using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class HabitPackageModelView
    {
        public HabitPackageModelView()
        {
            DateTime = DateTime.Today;
            Questions = new List<HabitEntryModelView>();
        }
        public DateTime DateTime { get; set; }
        public List<HabitEntryModelView> Questions { get; set; }
    }
}
