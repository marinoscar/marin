using Luval.Data;
using Luval.Data.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
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

        public static async Task<IActionResult> MicrosofAccountSignout(this Controller controller, string returnUrl)
        {
            //To remove all accounts, need this as well
            // return Redirect("https://login.microsoftonline.com/common/oauth2/v2.0/logout");

            var context = controller.HttpContext;
            var cookie = context.Request.Cookies.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Key)
                    && i.Key.ToLowerInvariant().Contains("aspnetcore"));

            if (!string.IsNullOrWhiteSpace(cookie.Key))
                context.Response.Cookies.Delete(cookie.Key);

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (string.IsNullOrWhiteSpace(returnUrl)) returnUrl = "/";
            return controller.Redirect(returnUrl);
        }
    }
}
