using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FastSpeedTestApi
{
    public class SpeedTestResult
    {
        public IEnumerable<PackageExecutionStatus> ExecutionStatus { get; set; }
        public TestUnit TestUnit { get; set; }
        public decimal DownloadSpeed { get; set; }
        public decimal BytesPerSecond { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
}
