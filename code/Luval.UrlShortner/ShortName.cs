using Luval.Common;
using Luval.Data.Attributes;
using Luval.Data.Entities;
using System;

namespace Luval.UrlShortner
{
    [TableName("UrlShortner")]
    public class ShortName : BaseAuditEntity
    {
        public ShortName() : base()
        {
            RecordCount = 0;
        }

        public string OriginalUri { get; set; }
        public long RecordCount { get; set; }

    }
}
