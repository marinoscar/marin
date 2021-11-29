using Luval.Data.Attributes;
using Luval.Data.Entities;
using System;

namespace Luval.UrlShortner
{
    [TableName("UrlShortner")]
    public class ShortName : StringKeyAuditEntity
    {
        public ShortName()
        {
            Id = Guid.NewGuid().ToString();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            RecordCount = 0;
        }

        public string OriginalUri { get; set; }
        public long RecordCount { get; set; }

    }
}
