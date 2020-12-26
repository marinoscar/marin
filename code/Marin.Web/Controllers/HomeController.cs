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
            //var properties = new AuthenticationProperties() {  };
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
            var properties = new AuthenticationProperties() { RedirectUri = Url.Action("Index") };
            
            var cookie = HttpContext.Request.Cookies.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Key) 
                    && i.Key.ToLowerInvariant().Contains("aspnetcore"));
            
            if(!string.IsNullOrWhiteSpace(cookie.Key))
                HttpContext.Response.Cookies.Delete(cookie.Key);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                Debug.WriteLine("Not authenticated");

            return Redirect(Url.Action("Index"));
        }
    }
}
