using Luval.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Luval.Web.Security
{
    public class MicrosoftUser : ExternalUser, IExternalUser, IStringKeyRecord
    {
        public MicrosoftUser() : base()
        {

        }

        public MicrosoftUser(ClaimsPrincipal principal) : this(principal.Claims)
        {
        }

        public MicrosoftUser(IEnumerable<Claim> claims) : base()
        {
            if (!claims.Any()) return;
            ProviderName = claims.First().OriginalIssuer;
            ProviderKey = GetClaimValue(claims, "nameidentifier");
            DisplayName = GetClaimValue(claims, "name");
            FirstName = GetClaimValue(claims, "givenname");
            LastName = GetClaimValue(claims, "surname");
            Email = GetClaimValue(claims, "emailaddress");
        }

        private string GetClaimValue(IEnumerable<Claim> claims, string shortName)
        {
            var longName = string.Format("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/{0}", shortName);
            var c = claims.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Type) && i.Type.ToLowerInvariant().Equals(longName));
            if (c == null) return null;
            return c.Value;
        }
    }
}
