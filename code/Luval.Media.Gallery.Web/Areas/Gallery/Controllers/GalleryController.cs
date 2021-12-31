using Luval.Common.Security;
using Luval.Data.Extensions;
using Luval.Media.Gallery.Entities;
using Luval.Web.Common;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Luval.Media.Gallery.Web.Areas.Gallery.Controllers
{
    [Area("Gallery"), Authorize]
    public class GalleryController : Controller
    {

        protected OAuthAuthoizationOptions AuthorizationOptions { get; private set; }
        protected IGraphAutenticationRepository GraphAuthenticationRepository { get; set; }
        protected IApplicationUserRepository UserRepository { get; private set; }
        protected ILogger<GalleryController> Logger { get; private set; }

        public GalleryController(IGraphAutenticationRepository graphAuthRepo, OAuthAuthoizationOptions authoizationOptions, IApplicationUserRepository userRepository, ILogger<GalleryController> logger)
        {
            AuthorizationOptions = authoizationOptions;
            GraphAuthenticationRepository = graphAuthRepo;
            UserRepository = userRepository;
            Logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private MicrosoftGraphCodeFlowHelper CreateHelper(HttpRequest httpRequest)
        {
            var scopes = "openid offline_access Files.ReadWrite Files.ReadWrite.All Files.ReadWrite.AppFolder Files.ReadWrite.Selected User.Read";
            return new MicrosoftGraphCodeFlowHelper(AuthorizationOptions.ClientId, AuthorizationOptions.ClientSecret, scopes, Request); ;
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
            var response = await graphHelper.GetTokenResponseAsync(code, "Gallery/Token", cancellationToken);
            var user = await UserRepository.GetUserAsync(User);
            var principal = await graphHelper.GetGraphPrincipalAsync(response.AccessToken, cancellationToken);
            await GraphAuthenticationRepository.CreateOrUpdateAsync(response, principal, user.Id, cancellationToken);

            if (Debugger.IsAttached) Logger.LogTrace("TOKEN: {0}", response.AccessToken);

            return View();
        }
    }
}
