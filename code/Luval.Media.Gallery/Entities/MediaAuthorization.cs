using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class MediaAuthorization : BaseAuditEntity
    {
        public string IdToken { get; set; }
        public string AccountEmail { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public double RefreshExpiration { get; set; }
        public DateTime UtcExpiration { get; set; }
        public string Scope { get; set; }
    }
}
