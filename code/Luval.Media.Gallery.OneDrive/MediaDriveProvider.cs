﻿using Azure.Identity;
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

        public EventHandler<DriveItemEventArgs> DriveItemProcessed;
        public EventHandler<DriveItemEventArgs> MediaItemProcessed;
        public EventHandler<DriveItemEventArgs> FolderItemProcessed;

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

        public async Task<IEnumerable<MediaItem>> GetMediaItemsFromDriveAsync(CancellationToken cancellationToken)
        {
            var res = new List<MediaItem>();
            var root = await _client.Me.Drive.Root.Request().GetAsync(cancellationToken);
            if (root != null)
            {
                OnDriveItemProcessed(root);
                res.AddRange(await GetMediaItemsFromDriveItemAsync(root.Id, false, cancellationToken));
            }
            return res;
        }

        public async Task<IEnumerable<MediaItem>> GetMediaItemsFromDriveItemAsync(string driveItemId, bool isRecursive, CancellationToken cancellationToken)
        {
            
            var res = new List<MediaItem>();
            var items = await _client.Me.Drive.Items[driveItemId].Children.Request().Expand("thumbnails").GetAsync(cancellationToken);
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
                        res.AddRange(await GetMediaItemsFromDriveItemAsync(item.Id, isRecursive, cancellationToken));
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
