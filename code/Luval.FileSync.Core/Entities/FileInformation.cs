using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Entities
{
    public class FileInformation
    {
        public FileInformation()
        {
            Id = CreateID();
            UtcFileCreatedOn = DateTime.UtcNow;
            UtcFileModifiedOn = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string? NameInDevice { get; set; }
        public string? Hash { get; set; }
        public string? ImageHash { get; set; }
        public string? Type { get; set; }
        public string? Format { get; set; }
        public DateTime UtcFileCreatedOn { get; set; }
        public DateTime UtcFileModifiedOn { get; set; }
        public DateTime? UtcImageTakenOn { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public float? Altitude { get; set; }
        public string? Country { get; set; }
        public string? Region1 { get; set; }
        public string? Region2 { get; set; }
        public string? Region3 { get; set; }
        public string? City { get; set; }

        private static string CreateID()
        {
            var id = "";
            lock (id)
            {
                Thread.Sleep(50);
                var d = DateTime.UtcNow;
                var rnd = new Random().Next(0, 1000);
                id = string.Format("{0}-{1}-{2}",
                    d.ToString("yyyyMMddHHmmss"),
                    d.Millisecond.ToString().PadLeft(4, '0'),
                    rnd.ToString().PadLeft(4, '0'));

            }
            return id;
        }


    }
}
