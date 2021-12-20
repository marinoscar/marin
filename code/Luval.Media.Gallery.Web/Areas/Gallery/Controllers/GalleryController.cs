using Luval.Common.Security;
using Luval.Data.Extensions;
using Luval.Web.Common;
using Luval.Web.Common.GraphApi;
using Luval.Web.Security;
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
    [Area("Gallery"), Authorize]
    public class GalleryController : Controller
    {

        private OAuthAuthoizationOptions _authoizationOptions;
        private ISafeItemRepository _safeItemRepository;
        private IApplicationUserRepository _userRepo;

        public GalleryController(ISafeItemRepository safeItemRepository, OAuthAuthoizationOptions authoizationOptions, IApplicationUserRepository userRepository)
        {
            _authoizationOptions = authoizationOptions;
            _safeItemRepository = safeItemRepository;
            _userRepo = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        private MicrosoftGraphCodeFlowHelper CreateHelper(HttpRequest httpRequest)
        {
            var scopes = "openid offline_access Files.ReadWrite Files.ReadWrite.All Files.ReadWrite.AppFolder Files.ReadWrite.Selected User.Read";
            return new MicrosoftGraphCodeFlowHelper(_authoizationOptions.ClientId, _authoizationOptions.ClientSecret, scopes, Request); ;
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
            var user = await _userRepo.GetUserAsync(User);
            await PersistCredentialsAsync(response, user.Id, cancellationToken);
            return View();
        }

        private async Task PersistCredentialsAsync(GraphTokenResponse graphToken, string userId, CancellationToken cancellationToken)
        {
            await _safeItemRepository.AddOrUpdateAsync("{0}:Token".FormatInvariant(graphToken.UserPrincipalName), graphToken.AccessToken, userId, cancellationToken);
            await _safeItemRepository.AddOrUpdateAsync("{0}:RefreshToken".FormatInvariant(graphToken.UserPrincipalName), graphToken.RefreshToken, userId, cancellationToken);
            await _safeItemRepository.AddOrUpdateAsync("{0}:TokenId".FormatInvariant(graphToken.UserPrincipalName), graphToken.IdToken, userId, cancellationToken);
        }
    }
}
