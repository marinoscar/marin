using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveItem
    {
        public DateTimeOffset? createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTimeOffset? lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public decimal? size { get; set; }
        public string webUrl { get; set; }
        public DriveEntity createdBy { get; set; }
        public DriveEntity lastModifiedBy { get; set; }
        public GraphEntity user { get; set; }
        public DriveReference parentReference { get; set; }
        public DriveFileSystemInfo fileSystemInfo { get; set; }
        public DriveFolder folder { get; set; }
        public DriveFile file { get; set; }
        public DriveVideo video { get; set; }
        public DriveGeoLocation location { get; set; }
        public DriveImage image { get; set; }
        public DrivePhoto photo { get; set; }
    }
}
