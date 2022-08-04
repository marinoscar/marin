using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class FileTransfer
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Device { get; set; }
        public string Hash { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdateOn { get; set; }
        public DateTime UtcTakenOn { get; set; }
        public Stream Stream { get; set; }

    }
}
