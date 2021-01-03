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
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private static readonly IDictionary<string, ApplicationUser> _userByEmail = new Dictionary<string, ApplicationUser>();

        public ApplicationUserRepository(IEntityAdapterFactory adapterFactory)
        {
            AdapterFactory = adapterFactory;
        }

        protected virtual IEntityAdapterFactory AdapterFactory { get; private set; }

        public async Task ValidateAndUpdateUserAccess(ClaimsPrincipal claimsPrincipal)
        {
            var email = claimsPrincipal.GetEmail();
            var userAdapter = AdapterFactory.Create<ApplicationUser, string>();
            var user = await GetUserByMailAsync(email);
            if (user == null) throw new AuthenticationException("Email {0} is not authorized".Fi(email));
            var appRoleClaims = new List<Claim>();
            foreach (var role in user.Roles)
            {
                appRoleClaims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName, "string", "Application"));
            }
            claimsPrincipal.AddIdentity(new ClaimsIdentity(appRoleClaims));
            await UpdateUser(user, claimsPrincipal, userAdapter);
        }

        public async Task<ApplicationUser> GetUserByMailAsync(string email)
        {
            if (_userByEmail.ContainsKey(email)) return _userByEmail[email];
            var userAdapter = AdapterFactory.Create<ApplicationUser, string>();
            var user = (await userAdapter.ReadAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null) throw new AuthenticationException("Email {0} is not authorized".Fi(email));
            user = await userAdapter.ReadAsync(user.Id, EntityLoadMode.Eager);
            await LoadRoles(user);
            _userByEmail[email] = user;
            return user;
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

        private async Task UpdateUser(ApplicationUser user, ClaimsPrincipal principal, IEntityAdapter<ApplicationUser, string> adapter)
        {
            user.DisplayName = GetClaimValue(ClaimTypes.GivenName, principal);
            user.FirstName = GetClaimValue(ClaimTypes.Name, principal);
            user.LastName = GetClaimValue(ClaimTypes.Surname, principal);
            user.ProviderKey = GetClaimValue(ClaimTypes.NameIdentifier, principal);
            user.UtcUpdatedOn = DateTime.UtcNow;
            await adapter.UpdateAsync(user);
        }

        private string GetClaimValue(string type, ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(type);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) return string.Empty;
            return claim.Value;
        }
    }
}
