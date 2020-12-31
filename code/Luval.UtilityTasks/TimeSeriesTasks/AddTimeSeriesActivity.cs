using Luval.Data;
using Luval.UtilityTasks.Models;
using Luval.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UtilityTasks.TimeSeriesTasks
{
    public class AddTimeSeriesActivity : ActivityTask
    {
        public AddTimeSeriesActivity() : base("Add Time Series", "MARIN-002")
        {

        }

        public string ConnectionString { get; set; }
        public string TimeSeriesMemoryLocation { get; set; }

        protected override void DoActivity(SessionContext context)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString)) throw new ArgumentNullException(nameof(ConnectionString));
            if (string.IsNullOrWhiteSpace(TimeSeriesMemoryLocation)) throw new ArgumentNullException(nameof(TimeSeriesMemoryLocation));
            var timeSeries = context.GetData<IEnumerable<TimeSeries>>(TimeSeriesMemoryLocation);
            if (timeSeries == null) throw new ArgumentNullException(nameof(timeSeries));
            if (!timeSeries.Any()) throw new ArgumentOutOfRangeException(nameof(timeSeries));

            var db = new SqlServerDatabase(ConnectionString);
            if (!db.TryConnection()) throw new ArgumentException(nameof(ConnectionString), "Unable to connect with the provided value");
            var sb = new StringBuilder();
            foreach (var ts in timeSeries)
            {
                sb.AppendLine(ts.ToSqlInsert());
            }
            db.ExecuteNonQuery(sb.ToString());
        }


    }
}
