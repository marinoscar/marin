using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FastSpeedTestApi
{
    public class PackageExecutionStatus
    {
        public TaskStatus TaskStatus { get; set; }
        public PackageResult PackageResult { get; set; }
    }
}
