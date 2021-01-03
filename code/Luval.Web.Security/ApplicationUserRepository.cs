using Luval.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ApplicationUserRepository
    {
        public ApplicationUserRepository(IEntityAdapterFactory adapterFactory)
        {
            AdapterFactory = adapterFactory;
        }

        protected virtual IEntityAdapterFactory AdapterFactory { get; private set; }

        public async Task ValidateAndUpdateUserAccess(ClaimsPrincipal claimsPrincipal)
        {
            var emailClaim = claimsPrincipal.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email);
            if(emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value))
                throw new AuthenticationException("Claims principal does not provide a valid email claim");
            var email = emailClaim.Value;
            var userAdapter = AdapterFactory.Create<ApplicationUser, string>();
            var user = (await userAdapter.ReadAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null) throw new AuthenticationException("Email {0} is not authorized".Fi(email));
            user = await userAdapter.ReadAsync(user.Id, EntityLoadMode.Eager);
            await LoadRoles(user);
            var appRoleClaims = new List<Claim>();
            foreach (var role in user.Roles)
            {
                appRoleClaims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName, "string", "Application"));
            }
            claimsPrincipal.AddIdentity(new ClaimsIdentity(appRoleClaims));
            UpdateUser(user, claimsPrincipal, userAdapter);
        }

        private async Task LoadRoles(ApplicationUser user)
        {
            var roleAdapter = this.AdapterFactory.Create<ApplicationRole, string>();
            foreach (var userRole in user.Roles)
            {
                userRole.User = user;
                userRole.Role = await roleAdapter.ReadAsync(userRole.ApplicationRoleId);
            }
        }

        private void UpdateUser(ApplicationUser user, ClaimsPrincipal principal, IEntityAdapter<ApplicationUser, string> adapter)
        {
            user.DisplayName = GetClaimValue(ClaimTypes.GivenName, principal);
            user.FirstName = GetClaimValue(ClaimTypes.Name, principal);
            user.LastName = GetClaimValue(ClaimTypes.Surname, principal);
            user.ProviderKey = GetClaimValue(ClaimTypes.NameIdentifier, principal);
            user.UtcUpdatedOn = DateTime.UtcNow;
            adapter.Update(user);
        }

        private string GetClaimValue(string type, ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(type);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) return string.Empty;
            return claim.Value;
        }
    }
}
