using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveFileSystemInfo
    {
        public DateTimeOffset? createdDateTime { get; set; }
        public DateTimeOffset? lastModifiedDateTime { get; set; }
    }
}
