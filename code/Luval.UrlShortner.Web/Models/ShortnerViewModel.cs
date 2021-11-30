using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web.Models
{
    public class ShortnerViewModel
    {
        public bool Success { get; set; }
        public bool IsDuplicate { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}
