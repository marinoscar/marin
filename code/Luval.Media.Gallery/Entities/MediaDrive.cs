using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class MediaDrive : BaseAuditEntity
    {
        public string DriveId { get; set; }
        public string DriveType { get; set; }
        public string DrivePath { get; set; }
        public string Name { get; set; }
        public bool LookInChildren { get; set; }
        public string Provider { get; set; }
        public string OnwerAccountEmail { get; set; }
    }
}
