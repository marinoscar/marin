using Marin.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Luval.Web.Security;
using Microsoft.AspNetCore.Identity;

namespace Marin.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                Debug.WriteLine("Not authenticated");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Authentication

        [AllowAnonymous, HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous, HttpGet("login/{provider}")]
        public IActionResult LoginExternal([FromRoute] string provider, [FromQuery] string returnUrl)
        {
            if (User != null && User.Identities.Any(identity => identity.IsAuthenticated))
                return RedirectToAction("", "Home");

            string domainUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            returnUrl = string.IsNullOrEmpty(returnUrl) ? domainUrl : returnUrl;
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
            return new ChallengeResult(provider, authenticationProperties);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var scheme = User.Claims.FirstOrDefault(c => c.Type == ".AuthScheme").Value;
            string domainUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            switch (scheme.ToLowerInvariant())
            {
                case "cookies":
                    await HttpContext.SignOutAsync();
                    return Redirect("/");
                case "google":
                    await HttpContext.SignOutAsync();
                    var redirect = $"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue={domainUrl}";
                    return Redirect(redirect);
                case "microsoft":
                    await HttpContext.SignOutAsync();
                    return Redirect("/");
                default:
                    return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, scheme });
            }
        }

        [AllowAnonymous, HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }
        #endregion
    }
}
