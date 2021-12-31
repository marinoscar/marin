using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Web.Areas.Gallery.Controllers
{
    [Area("Gallery"), AllowAnonymous]
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;
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

            // If there is a validation token in the query string,
            // send it back in a 200 OK text/plain response
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

            // Validate any tokens in the payload
            //var areTokensValid = await AreTokensValid(notifications, _tenantIds, _appIds);
            //if (!areTokensValid) return Unauthorized();

            //// Process non-encrypted notifications first
            //// These will be notifications for user mailbox
            //var messageNotifications = new Dictionary<string, ChangeNotification>();
            //foreach (var notification in notifications.Value.Where(n => n.EncryptedContent == null))
            //{
            //    // Find the subscription in our store
            //    var subscription = _subscriptionStore
            //        .GetSubscriptionRecord(notification.SubscriptionId.ToString());

            //    // If this isn't a subscription we know about, or if client state doesnt' match,
            //    // ignore it
            //    if (subscription != null && subscription.ClientState == notification.ClientState)
            //    {
            //        _logger.LogInformation($"Received notification for: {notification.Resource}");
            //        // Add notification to list to process. If there is more than
            //        // one notification for a given resource, we'll only process it once
            //        messageNotifications[notification.Resource] = notification;
            //    }
            //}

            //// Since resource data is not included in these notifications,
            //// use Microsoft Graph to get the messages
            //await GetMessagesAsync(messageNotifications.Values);

            //// Process encrypted notifications
            //var clientNotifications = new List<ClientNotification>();
            //foreach (var notification in notifications.Value.Where(n => n.EncryptedContent != null))
            //{
            //    // Decrypt the encrypted payload using private key
            //    var chatMessage = await notification.EncryptedContent.DecryptAsync<ChatMessage>((id, thumbprint) => {
            //        return _certificateService.GetDecryptionCertificate();
            //    });

            //    // Add a SignalR notification for this message to the list
            //    clientNotifications.Add(new ClientNotification(new
            //    {
            //        Sender = chatMessage.From?.User?.DisplayName ?? "UNKNOWN",
            //        Message = chatMessage.Body?.Content ?? ""
            //    }));
            //}

            //// Send SignalR notifications
            //if (clientNotifications.Count > 0)
            //{
            //    await _hubContext.Clients.All.SendAsync("showNotification", clientNotifications);
            //}

            //// Return 202 to Graph to confirm receipt of notification.
            //// Not sending this will cause Graph to retry the notification.
            return Accepted();
        }

        [HttpPost, AllowAnonymous, Route("Gallery/CreateSubscription")]
        public async Task<IActionResult> CreateSubscription()
        {

        }


    }
}
