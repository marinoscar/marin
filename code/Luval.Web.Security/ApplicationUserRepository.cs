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

        public async Task<ApplicationUser> GetUserByMailAsync(string email)
        {
            if (_userByEmail.ContainsKey(email)) return _userByEmail[email];
            var userAdapter = UnitOfWorkFactory.Create<ApplicationUser, string>();
            var user = (await userAdapter.Entities.Query.GetAsync(u => u.Email == email, CancellationToken.None)).FirstOrDefault();
            if (user == null) throw new AuthenticationException("Email {0} is not authorized".Fi(email));
            user = await userAdapter.Entities.Query.GetAsync(user.Id, EntityLoadMode.Eager, CancellationToken.None);
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
                userRole.Role = await unitOfWork.Entities.Query.GetAsync(userRole.ApplicationRoleId, CancellationToken.None);
            }
        }

        private async Task UpdateUserAsync(ApplicationUser user, ClaimsPrincipal principal, IUnitOfWork<ApplicationUser, string> unitofWork, CancellationToken cancellationToken)
        {
            user.DisplayName = GetClaimValue(ClaimTypes.Name, principal);
            user.FirstName = GetClaimValue(ClaimTypes.GivenName, principal);
            user.LastName = GetClaimValue(ClaimTypes.Surname, principal);
            user.ProviderKey = GetClaimValue(ClaimTypes.NameIdentifier, principal);
            user.UtcUpdatedOn = DateTime.UtcNow;
            unitofWork.Entities.Update(user);
            await unitofWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<ApplicationUser> CreateOrUpdateExternalUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await this.GetUserAsync(principal);
            if (user == null) { user = await CreateExternalUserAsync(principal, cancellationToken); }
            else
            {
                var claimsIdentity = principal.Identity as ClaimsIdentity;
                claimsIdentity.AddClaim(new Claim(SecurityConstants.AppUserIdFieldName, user.Id));
                var uom = UnitOfWorkFactory.Create<ApplicationUser, string>();
                if (user.Roles.Any())
                {
                    var roles = await uom.Entities.Query.GetRawAsync("SELECT AR.RoleName from ApplicationRole AR INNER JOIN ApplicationUserRole UR ON AR.Id = UR.ApplicationRoleId WHERE UR.ApplicationUserId =  {0}".FormatSql(user.Id).ToCmd(),
                    cancellationToken);
                    foreach (var role in roles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Convert.ToString(role["RoleName"])));
                    }
                }
                else
                    foreach (var role in user.Roles)
                        if (role.Role != null && !string.IsNullOrWhiteSpace(role.Role.RoleName))
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role.RoleName));
            }
            return user;
        }

        public async Task<ApplicationUser> CreateExternalUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser()
            {
                DisplayName = GetClaimValue(ClaimTypes.Name, principal),
                FirstName = GetClaimValue(ClaimTypes.GivenName, principal),
                LastName = GetClaimValue(ClaimTypes.Surname, principal),
                ProviderKey = GetClaimValue(ClaimTypes.NameIdentifier, principal),
            };
            var userUoW = UnitOfWorkFactory.Create<ApplicationUser, string>();
            var roleUoW = UnitOfWorkFactory.Create<ApplicationRole, string>();

            var role = (await roleUoW.Entities.Query.GetAsync(i => i.RoleName == SecurityConstants.VisitorRoleName, cancellationToken)).FirstOrDefault();
            if (role == null)
            {
                role = new ApplicationRole() { RoleName = SecurityConstants.VisitorRoleName, CreatedByUserId = user.Id, UpdatedByUserId = user.Id };
                await roleUoW.AddAndSaveAsync(role, cancellationToken);
            }

            user.Roles.Add(new ApplicationUserRole() { ApplicationUserId = user.Id, ApplicationRoleId = role.Id, CreatedByUserId = user.Id, UpdatedByUserId = user.Id });
            userUoW.Entities.Add(user);
            await userUoW.SaveChangesAsync(cancellationToken);
            var claimsIdentity = principal.Identity as ClaimsIdentity;
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
            claimsIdentity.AddClaim(new Claim(SecurityConstants.AppUserIdFieldName, user.Id));
            return user;
        }

        private string GetClaimValue(string type, ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(type);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) return string.Empty;
            return claim.Value;
        }
    }
}
