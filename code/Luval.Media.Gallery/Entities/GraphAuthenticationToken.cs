using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class GraphAuthenticationToken : BaseAuditEntity
    {
        public GraphAuthenticationToken() : base() { }

        public string UserId { get; set; }
        public string PrincipalEmail { get; set; }
        public string Token { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime UtcExpiration { get; set; }
    }
}
