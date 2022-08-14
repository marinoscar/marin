using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Entities
{
    public class FileInformation
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string LocationInDevice { get; set; }
        public string NameInDevice { get; set; }
        public string Hash { get; set; }
        public string ImageHash { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public DateTime UtcFileCreatedOn { get; set; }
        public DateTime UtcFileModifiedOn { get; set; }
        public DateTime? UtcImageTakenOn { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public float? Altitude { get; set; }
        public string Country { get; set; }
        public string Region1 { get; set; }
        public string Region2 { get; set; }
        public string Region3 { get; set; }
        public string City { get; set; }


    }
}
