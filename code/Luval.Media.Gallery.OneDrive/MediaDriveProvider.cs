using Azure.Identity;
using Luval.Data.Extensions;
using Luval.Media.Gallery.Entities;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class MediaDriveProvider
    {

        IAuthenticationProvider _authenticationProvider;
        GraphServiceClient _client;

        public event EventHandler<DriveItemEventArgs> DriveItemProcessed;
        public event EventHandler<DriveItemEventArgs> MediaItemProcessed;
        public event EventHandler <DriveItemEventArgs> FolderItemProcessed;

        protected virtual void OnDriveItemProcessed(DriveItem driveItem)
        {
            DriveItemProcessed?.Invoke(this, new DriveItemEventArgs(driveItem));
        }

        protected virtual void OnMediaItemProcessed(DriveItem driveItem)
        {
            MediaItemProcessed?.Invoke(this, new DriveItemEventArgs(driveItem));
        }

        protected virtual void OnFolderItemProcessed(DriveItem driveItem)
        {
            FolderItemProcessed?.Invoke(this, new DriveItemEventArgs(driveItem));
        }



        public MediaDriveProvider(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
            _client = new GraphServiceClient(_authenticationProvider);
        }

        public async Task<IEnumerable<GraphSubscription>> CreateAllSubscriptionAsync(string callbackUrl)
        {
            var res = new List<GraphSubscription>();
            res.Add(await SetupSubscriptionAsync(callbackUrl, "created,updated,deleted"));
            //res.Add(await SetupItemUpdatedSubscriptionAsync(callbackUrl));
            //res.Add(await SetupItemDeletedSubscriptionAsync(callbackUrl));
            return res;
        }

        public Task<GraphSubscription> SetupItemCreatedSubscriptionAsync(string callbackUrl)
        {
            return SetupSubscriptionAsync(callbackUrl, "created");
        }

        public Task<GraphSubscription> SetupItemUpdatedSubscriptionAsync(string callbackUrl)
        {
            return SetupSubscriptionAsync(callbackUrl, "updated");
        }

        public Task<GraphSubscription> SetupItemDeletedSubscriptionAsync(string callbackUrl)
        {
            return SetupSubscriptionAsync(callbackUrl, "deleted");
        }

        private async Task<GraphSubscription> SetupSubscriptionAsync(string callbackUrl, string changeType)
        {
            var subscription = new Subscription()
            {
                ChangeType = changeType,
                NotificationUrl = callbackUrl,
                ClientState = Guid.NewGuid().ToString(),
                IncludeResourceData = true,
                ExpirationDateTime = DateTime.UtcNow.AddMonths(6),
                Resource = "me/drive/root"
            };
            var newSubscription = await _client.Subscriptions
                    .Request().AddAsync(subscription);
            return newSubscription.ToGraphSubscription();
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsFromDriveAsync(CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var root = await _client.Me.Drive.Root.Request().GetAsync(cancellationToken);
            if (root != null)
            {
                OnDriveItemProcessed(root);
                if (root.IsFolder()) OnFolderItemProcessed(root);
                res.AddRange(await GetMediaItemsFromDriveItemAsync(root.Id, false, cancellationToken));
            }
            return res;
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsFromDriveItemAsync(string driveItemId, bool isRecursive, CancellationToken cancellationToken)
        {
            var rootItem = await _client.Me.Drive.Items[driveItemId].Request().GetAsync(cancellationToken);
            if (rootItem != null && rootItem.IsFolder()) OnFolderItemProcessed(rootItem);
            return await GetMediaItemsFromDriveItemAsync(rootItem, isRecursive, cancellationToken);
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsFromDriveItemAsync(DriveItem driveItem, bool isRecursive, CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var items = await _client.Me.Drive.Items[driveItem.Id].Children.Request().Expand("thumbnails").GetAsync(cancellationToken);
            var currentPage = items.CurrentPage;
            while (currentPage.Any())
            {
                foreach (var item in currentPage)
                {
                    OnDriveItemProcessed(item);
                    if (item.IsMediaItem())
                    {
                        OnMediaItemProcessed(item);
                        res.Add(item.ToMediaItem());
                    }
                    else if (item.IsFolder() && isRecursive)
                    {
                        OnFolderItemProcessed(item);
                        res.AddRange(await GetMediaItemsFromDriveItemAsync(item, isRecursive, cancellationToken));
                    }
                }
                if (items.NextPageRequest != null)
                {
                    items = await items.NextPageRequest.GetAsync(cancellationToken);
                    if (items != null) currentPage = items.CurrentPage;
                }
                else
                    currentPage.Clear();
            }
            return res;
        }
    }

    public class DriveItemEventArgs : EventArgs
    {
        public DriveItemEventArgs(DriveItem driveItem)
        {
            DriveItem = driveItem;
        }
        public DriveItem DriveItem { get; private set; }
    }
}
