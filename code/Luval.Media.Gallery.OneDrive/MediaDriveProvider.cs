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


        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveAsync(MediaDrive drive, CancellationToken cancellationToken)
        {
            //fix to the issue:
            //https://github.com/OneDrive/onedrive-api-docs/issues/1266#issuecomment-766941950

            var graphClient = new GraphServiceClient(_authenticationProvider);
            var drives = await graphClient.Me.Drive.Root.Children.Request().GetAsync(cancellationToken);

            return new List<MediaItem>();
        }

        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveAsync(CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var items = await _client.Me.Drive.Root.Children.Request().GetAsync(cancellationToken);
            var currentPage = items.CurrentPage;
            while (currentPage.Any())
            {
                foreach (var item in currentPage)
                {
                    if(item.IsMediaItem())
                        res.Add(item.ToMediaItem());
                }
                var newPage = await items.NextPageRequest.GetAsync(cancellationToken);
                if (newPage != null) currentPage = newPage.CurrentPage;
            }
            return res;
        }

        public Task<IEnumerable<MediaItem>> GetDriveItemChildren(DriveItem driveItem, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //var res = new List<MediaItem>();
            //var currentPage = items.CurrentPage;
            //while (currentPage.Any())
            //{
            //    foreach (var item in currentPage)
            //    {
            //        if (item.IsMediaItem())
            //            res.Add(item.ToMediaItem());
            //    }
            //    var newPage = await items.NextPageRequest.GetAsync(cancellationToken);
            //    if (newPage != null) currentPage = newPage.CurrentPage;
            //}
            //return res;
        }
    }
}
