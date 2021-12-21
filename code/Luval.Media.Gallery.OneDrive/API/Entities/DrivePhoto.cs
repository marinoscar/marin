using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DrivePhoto
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public double? exposureDenominator { get; set; }
        public double? exposureNumerator { get; set; }
        public double? focalLength { get; set; }
        public double? fNumber { get; set; }
        public double? iso { get; set; }
        public DateTimeOffset? takenDateTime { get; set; }
    }
}
