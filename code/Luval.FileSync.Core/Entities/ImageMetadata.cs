using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Entities
{
    public class ImageMetadata
    {
        public DateTime? UtcDateTaken { get; set; }
        public ImageGeoLocation GeoLocation { get; set; }

    }
}
