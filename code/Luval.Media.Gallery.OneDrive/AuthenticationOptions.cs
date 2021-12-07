using Luval.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class AuthenticationOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string ApiUrl { get { return "https://graph.microsoft.com/";  } }

        public string GetAuthority()
        {
            return "https://login.microsoftonline.com/{0}".FormatInvariant(TenantId);
        }

        public string[] GetDefaultScopes()
        {
            return new string[] { $"{ApiUrl}.default" };
        }
    }
}
