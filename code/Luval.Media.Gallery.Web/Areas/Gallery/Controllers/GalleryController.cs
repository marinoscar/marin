using Luval.Data.Extensions;
using Luval.Web.Common;
using Microsoft.AspNetCore.Authorization;
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

        MicrosoftGraphCodeFlowHelper _graphHelper;

        public GalleryController()
        {
            var scopes = "openid offline_access Files.ReadWrite Files.ReadWrite.All Files.ReadWrite.AppFolder Files.ReadWrite.Selected User.Read";
            _graphHelper = new MicrosoftGraphCodeFlowHelper("", "", scopes, Request);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("Gallery/Account")]
        public IActionResult Account()
        {
            var url = _graphHelper.GetCodeAuthorizationUrl("Gallery/Token");
            return Redirect(HttpUtility.HtmlEncode(url));
        }

        [HttpGet, Route("Gallery/Token")]
        public async Task<IActionResult> Token(string code, string state, CancellationToken cancellationToken)
        {
            var response = await _graphHelper.GetCodeAuthorizationAsync(code, "Gallery/Token", cancellationToken);
            return View();
        }
    }
}
