using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using Luval.Media.Gallery.OneDrive;
using Luval.Web.Common;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Web.Areas.Gallery.Controllers
{
    [Area("Gallery"), AllowAnonymous]
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IApplicationUserRepository _userRepository;

        public SubscriptionController(ILogger<SubscriptionController> logger, IUnitOfWorkFactory uowFactory, IApplicationUserRepository userRepository)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _userRepository = userRepository;
        }
        /// <summary>
        /// POST /listen
        /// </summary>
        /// <param name="validationToken">Optional. Validation token sent by Microsoft Graph during endpoint validation phase</param>
        /// <returns>IActionResult</returns>
        [HttpPost, AllowAnonymous, Route("Gallery/Subscription")]
        public async Task<IActionResult> Index([FromQuery] string validationToken = null)
        {
            _logger.LogInformation("Access the subscription controller");

            try
            {
                if (!string.IsNullOrEmpty(validationToken))
                {
                    _logger.LogInformation("Validation succesful");
                    return Ok(validationToken);
                }

                // Read the body
                using var reader = new StreamReader(Request.Body);
                var jsonPayload = await reader.ReadToEndAsync();
                _logger.LogInformation(jsonPayload);

                // Use the Graph client's serializer to deserialize the body
                var notifications = JsonConvert.DeserializeObject(jsonPayload);

                if (notifications == null) return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to complete the operation");
                throw;
            }

            return Accepted();
        }

        [HttpPost, AllowAnonymous, Route("Gallery/CreateSubscription")]
        public async Task<IActionResult> CreateSubscription(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(User);
                var subsRepo = new SubscriptionRepository(_uowFactory.Create<GraphSubscription, string>());
                var tokenRepo = new GraphAutenticationRepository(_uowFactory.Create<GraphAuthenticationToken, string>());
                _logger.LogInformation("Getting token");
                var token = await tokenRepo.GetByUserEmailAsync(user.Email, cancellationToken);
                var tokenProvider = new TokenAuthenticatorProvider(token.Token);
                var mediaProvider = new MediaDriveProvider(tokenProvider);
                var rootUrl = this.GetRequestRootUri().ToString();
                var callback = string.Format("{0}/Gallery/Subscription", rootUrl);
                _logger.LogInformation("Creating subscription for root: {0} and callback {1}", rootUrl, callback);
                var subs = await mediaProvider.CreateAllSubscriptionAsync(callback);
                foreach (var sub in subs)
                {
                    _logger.LogInformation("Persisting {0}", sub);
                    await subsRepo.CreateAsync(sub, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create subscription");
                throw;
            }
            
            return Ok();

        }


    }
}
