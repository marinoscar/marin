using Luval.Data.Extensions;
using Luval.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Luval.Media.Gallery.Web.Areas.Gallery.Controllers
{
    [Area("Gallery"), AllowAnonymous]
    public class GalleryController : Controller
    {


        public GalleryController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        private MicrosoftGraphCodeFlowHelper CreateHelper(HttpRequest httpRequest)
        {
            var scopes = "openid offline_access Files.ReadWrite Files.ReadWrite.All Files.ReadWrite.AppFolder Files.ReadWrite.Selected User.Read";
            return new MicrosoftGraphCodeFlowHelper("", "", scopes, Request); ;
        }

        [HttpGet, Route("Gallery/Account")]
        public IActionResult Account()
        {
            var graphHelper = CreateHelper(Request);
            var url = graphHelper.GetCodeAuthorizationUrl("Gallery/Token");
            return Redirect(url);
        }

        [HttpGet, Route("Gallery/Token")]
        public async Task<IActionResult> Token(string code, string state, CancellationToken cancellationToken)
        {
            var graphHelper = CreateHelper(Request);
            var response = await graphHelper.GetCodeAuthorizationAsync(code, "Gallery/Token", cancellationToken);
            return View();
        }
    }
}
