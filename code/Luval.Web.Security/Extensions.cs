using Luval.Data;
using Luval.Data.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public static class Extensions
    {

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.Email);
            if (claim == null) throw new ArgumentNullException("Email", "Claims principal does not have a valid email address");
            return claim.Value;
        }

        public static Task UpdatePrincipalOnSignIn(this CookieAuthenticationOptions options, CookieSigningInContext context)
        {
            return Task.Run(() => {
                var appUserRepo = context.HttpContext.RequestServices.GetService<IApplicationUserRepository>();
                var scheme = context.Properties.Items.Where(k => k.Key == SecurityConstants.AuthSchemePropertyName).FirstOrDefault();
                var token = context.Properties.Items.Where(k => k.Key == ".Token.access_token").FirstOrDefault();
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                var user = appUserRepo.CreateOrUpdateExternalUserAsync(context.Principal, CancellationToken.None).Result;
                claimsIdentity.AddClaim(new Claim(scheme.Key, scheme.Value));
                claimsIdentity.AddClaim(new Claim(token.Key, token.Value));
                if (user != null)
                    foreach (var role in user.Roles)
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role.RoleName));
            });
        }

        public static Task<ApplicationUser> GetUserAsync(this IApplicationUserRepository repo, ClaimsPrincipal principal)
        {
            return repo.GetUserByMailAsync(GetEmail(principal));
        }

        public static ApplicationUser GetUser(this IApplicationUserRepository repo, ClaimsPrincipal principal)
        {
            return repo.GetUserByMailAsync(GetEmail(principal)).Result;
        }

        public static void PrepareEntityForUpdate<TKey>(this IApplicationUserRepository repo, ClaimsPrincipal principal, IAuditableEntity<TKey> entity)
        {
            PrepareForUpdate(entity, GetUser(repo, principal));
        }

        public static void PrepareEntityForInsert<TKey>(this IApplicationUserRepository repo, ClaimsPrincipal principal, IAuditableEntity<TKey> entity)
        {
            PrepareForInsert(entity, GetUser(repo, principal));
        }

        public static void PrepareForUpdate<TKey>(this IAuditableEntity<TKey> entity, IApplicationUser user)
        {
            entity.UtcUpdatedOn = DateTime.UtcNow;
            entity.UpdatedByUserId = user.Id;
        }

        public static void PrepareForInsert<TKey>(this IAuditableEntity<TKey> entity, IApplicationUser user)
        {
            entity.UtcUpdatedOn = DateTime.UtcNow;
            entity.UpdatedByUserId = user.Id;
            entity.CreatedByUserId = entity.UpdatedByUserId;
            entity.UtcCreatedOn = entity.UtcUpdatedOn;
        }
    }
}
