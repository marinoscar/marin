using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveFile
    {
        public string mimeType { get; set; }
        public DriveHashes hashes { get; set; }
    }
}
