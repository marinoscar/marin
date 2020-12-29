using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Core
{
    internal class ActivityName : IActivityName
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public Type ActivityType { get; set; }

        public override string ToString()
        {
            return string.Join(",", new[] { Id, DisplayName, Convert.ToString(ActivityType) });
        }
    }
}
