using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveFolder
    {
        public int? childCount { get; set; }
        public DriveFolderView view { get; set; }
    }
}
