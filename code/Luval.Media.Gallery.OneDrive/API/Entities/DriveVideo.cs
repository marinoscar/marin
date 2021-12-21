using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive.API.Entities
{
    public class DriveVideo
    {
        public double? bitrate { get; set; }
        public double? duration { get; set; }
        public double? height { get; set; }
        public double? width { get; set; }
        public double? audioBitsPerSample { get; set; }
        public double? audioChannels { get; set; }
        public string audioFormat { get; set; }
        public double? audioSamplesPerSecond { get; set; }
        public string fourCC { get; set; }
        public double? frameRate { get; set; }

    }
}
