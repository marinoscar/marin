using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class MediaItem : BaseAuditEntity
    {
        public MediaItem() : base()
        {

        }

        public string MediaId { get; set; }
        public string MediaType { get; set; }
        public string MediaMimeType { get; set; }
        public string Hash { get; set; }
        public string DeviceName { get; set; }
        public string DrivePath { get; set; }
        public DateTimeOffset? MediaCreatedTime { get; set; }
        public DateTimeOffset? MediaUpdatedTime { get; set; }
        public string FileName { get; set; }
        public decimal Size { get; set; }
        public string WebUrl { get; set; }
        public string CreatedByApp { get; set; }
        public string CreatedByUser { get; set; }
        public DateTimeOffset? FileCreationDateTime { get; set; }
        public DateTimeOffset? FileUpdateDateTime { get; set; }
        public DateTimeOffset? MediaTakenDateTime { get; set; }
        public double? LocationAltitute { get; set; }
        public double? LocationLatitude { get; set; }
        public double? Locationlongitude { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string SubRegion { get; set; }
        public string PostalCode { get; set; }
        public string ThumbSmall { get; set; }
        public string ThumbMid { get; set; }
        public string ThumbLarge { get; set; }

    }
}
