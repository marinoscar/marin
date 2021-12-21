using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveHashes
    {
        public string quickXorHash { get; set; }
        public string sha1Hash { get; set; }
        public string sha256Hash { get; set; }
    }
}
