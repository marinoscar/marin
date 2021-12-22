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

        public MediaDriveProvider(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
            _client = new GraphServiceClient(_authenticationProvider);
        }

        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveAsync(CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var root = await _client.Me.Drive.Root.Request().GetAsync(cancellationToken);
            if (root != null)
                res.AddRange(await GetItemsFromDriveItemAsync(root.Id, false, cancellationToken));
            return res;
        }

        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveItemAsync(string driveItemId, bool isRecursive, CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var items = await _client.Me.Drive.Items[driveItemId].Children.Request().GetAsync(cancellationToken);
            var currentPage = items.CurrentPage;
            while (currentPage.Any())
            {
                foreach (var item in currentPage)
                {
                    if (item.IsMediaItem())
                        res.Add(item.ToMediaItem());
                    else if (item.IsFolder() && isRecursive)
                        res.AddRange(await GetItemsFromDriveItemAsync(item.Id, isRecursive, cancellationToken));
                }
                if (items.NextPageRequest != null)
                {
                    var newPage = await items.NextPageRequest.GetAsync(cancellationToken);
                    if (newPage != null) currentPage = newPage.CurrentPage;
                }
                else
                    currentPage.Clear();
            }
            return res;
        }
    }
}
