using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.UtilityTasks.Models
{
    public class TimeSeries
    {

        public TimeSeries(): this(null, null, DateTime.UtcNow)
        {

        }

        public TimeSeries(string dataLabel, decimal? value) : this(dataLabel, value, DateTime.UtcNow)
        {
        }

        public TimeSeries(string dataLabel, decimal? value, DateTime utcTime) : this(dataLabel, value, null, utcTime)
        {
        }

        public TimeSeries(string dataLabel, decimal? value, string textValue, DateTime utcTime)
        {
            DataLabel = dataLabel;
            NumericValue = value;
            StringValue = textValue;
            UtcTimestamp = utcTime;
        }

        [PrimaryKey, IdentityColumn]
        public long Id { get; set; }
        public string DataLabel { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public decimal? NumericValue { get; set; }
        public string StringValue { get; set; }
        public string CreatedByUserProfileId { get; set; }

        public string ToSqlInsert()
        {
            return string.Format("INSERT INTO [TimeSeries] ([DataLabel], [UtcTimestamp], [NumericValue], [StringValue], [CreatedByUserProfileId]) VALUES ({0},{1},{2},{3},{4});",
                DataLabel.ToSql(), UtcTimestamp.ToSql(), NumericValue.ToSql(), StringValue.ToSql(), CreatedByUserProfileId.ToSql());
        }

    }
}
