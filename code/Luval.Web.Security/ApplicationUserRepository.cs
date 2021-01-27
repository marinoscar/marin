using Luval.Data;
using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private static readonly IDictionary<string, ApplicationUser> _userByEmail = new Dictionary<string, ApplicationUser>();

        public ApplicationUserRepository(IUnitOfWorkFactory uowFactory)
        {
            UnitOfWorkFactory = uowFactory;
        }

        protected virtual IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        public async Task ValidateAndUpdateUserAccess(ClaimsPrincipal claimsPrincipal)
        {
            var email = claimsPrincipal.GetEmail();
            var userAdapter = UnitOfWorkFactory.Create<ApplicationUser, string>();
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
            var userAdapter = UnitOfWorkFactory.Create<ApplicationUser, string>();
            var user = (await userAdapter.Entities.GetAsync(u => u.Email == email, CancellationToken.None)).FirstOrDefault();
            if (user == null) throw new AuthenticationException("Email {0} is not authorized".Fi(email));
            user = await userAdapter.Entities.GetAsync(user.Id, EntityLoadMode.Eager, CancellationToken.None);
            await LoadRoles(user);
            _userByEmail[email] = user;
            return user;
        }

        private async Task LoadRoles(ApplicationUser user)
        {
            var unitOfWork = this.UnitOfWorkFactory.Create<ApplicationRole, string>();
            foreach (var userRole in user.Roles)
            {
                userRole.User = user;
                userRole.Role = await unitOfWork.Entities.GetAsync(userRole.ApplicationRoleId, CancellationToken.None);
            }
        }

        private async Task UpdateUser(ApplicationUser user, ClaimsPrincipal principal, IUnitOfWork<ApplicationUser, string> unitofWork)
        {
            user.DisplayName = GetClaimValue(ClaimTypes.Name, principal);
            user.FirstName = GetClaimValue(ClaimTypes.GivenName, principal);
            user.LastName = GetClaimValue(ClaimTypes.Surname, principal);
            user.ProviderKey = GetClaimValue(ClaimTypes.NameIdentifier, principal);
            user.UtcUpdatedOn = DateTime.UtcNow;
            unitofWork.Entities.Update(user);
            await unitofWork.SaveChangesAsync(CancellationToken.None);
        }

        private string GetClaimValue(string type, ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(type);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) return string.Empty;
            return claim.Value;
        }
    }
}
