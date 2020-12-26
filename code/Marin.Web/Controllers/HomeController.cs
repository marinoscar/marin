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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous, Route("signin-microsoft")]
        public IActionResult MicrosoftSigin()
        {
            var properties = new AuthenticationProperties() { RedirectUri = Url.Action("AuthResponse") };
            return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        [AllowAnonymous, Route("auth-response")]
        public async Task<IActionResult> AuthResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(c => new
            {
                Issuer = c.Issuer,
                OriginalIssuer = c.OriginalIssuer,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            }).ToList();
            return Json(claims);
        }

        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            return await this.MicrosofAccountSignout(returnUrl);
        }
    }
}
