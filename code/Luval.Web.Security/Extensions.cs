using Luval.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public static class Extensions
    {
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

        public static AuthenticationBuilder AddExternalMarinSignIn<TUser, TRole>(this IServiceCollection services, Database database) 
            where TUser : ApplicationUser 
            where TRole : ApplicationRole
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped((s) =>
            {
                return new SqlEntityAdapter<TUser>(database, new SqlServerDialectFactory());
            });

            services.AddScoped((s) =>
            {
                return new SqlEntityAdapter<ApplicationRole>(database, new SqlServerDialectFactory());
            });

            services.AddIdentity<TUser, ApplicationRole>()
                .AddUserStore<ApplicationUserStore<TUser>>()
                .AddUserManager<UserManager<TUser>>()
                .AddRoleStore<ApplicationRoleStore<TRole>>()
                .AddRoleManager<RoleManager<TRole>>();

            return new AuthenticationBuilder(services);
        }
    }
}
