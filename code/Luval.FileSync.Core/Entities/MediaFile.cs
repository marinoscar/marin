using Luval.DataStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Entities
{
    public class MediaFile : IStringIdEntity
    {
        public MediaFile()
        {
            Id = CreateID();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            DeviceName = Environment.MachineName;
        }

        public string Id { get; set; }
        public string? LocationInDevice { get; set; }
        public string? DeviceName { get; set; }
        public string? Hash { get; set; }
        public string? ImageHash { get; set; }
        public string? Format { get; set; }
        public DateTime UtcFileCreatedOn { get; set; }
        public DateTime UtcFileModifiedOn { get; set; }
        public DateTime? UtcImageTakenOn { get; set; }
        public DateTime? UtcUploadedOn { get; set; }
        public DateTime? UtcDeletedOn { get; set; }
        public DateTime? UtcProcessedOn { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Altitude { get; set; }
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
