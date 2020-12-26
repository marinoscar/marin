using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
