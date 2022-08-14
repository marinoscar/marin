using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Entities
{
    public class Device
    {
        public Device()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? OS { get; set; }
    }
}
