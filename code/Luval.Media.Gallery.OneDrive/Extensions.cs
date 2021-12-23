using Luval.Media.Gallery.Entities;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public static class Extensions
    {
        public static bool IsMediaItem(this DriveItem item)
        {
            return (item != null && item.File != null &&
                !string.IsNullOrWhiteSpace(item.File.MimeType) &&
                (item.File.MimeType.ToLowerInvariant().Contains("video") || item.File.MimeType.ToLowerInvariant().Contains("image")));
        }

        public static bool IsFolder(this DriveItem item)
        {
            return item.Folder != null;
        }

        public static MediaItem ToMediaItem(this DriveItem item)
        {
            var res = new MediaItem() {
                MediaId = item.Id,
                WebUrl = item.WebUrl,
                MediaCreatedTime = item.CreatedDateTime,
                MediaUpdatedTime = item.LastModifiedDateTime,
                Name = item.Name,
                Size = Convert.ToDecimal(item.Size),
            };
            if(item.ParentReference != null)
            {
                res.DrivePath = item.ParentReference.Path;
                res.DriveId = item.ParentReference.DriveId;
            }
            if(item.FileSystemInfo != null)
            {
                res.FileCreationDateTime = item.FileSystemInfo.CreatedDateTime;
                res.FileUpdateDateTime = item.FileSystemInfo.LastModifiedDateTime;
            }
            if(item.File != null)
            {
                res.MediaMimeType = item.File.MimeType;
                res.Hash = item.File.Hashes.Sha1Hash;
                if (!string.IsNullOrWhiteSpace(item.File.MimeType) && item.File.MimeType.ToLowerInvariant().Contains("video"))
                    res.MediaType = "video";
                if (!string.IsNullOrWhiteSpace(item.File.MimeType) && item.File.MimeType.ToLowerInvariant().Contains("image"))
                    res.MediaType = "image";
            }
            if(item.Photo != null)
            {
                res.MediaTakenDateTime = item.Photo.TakenDateTime;
            }
            if(item.Location != null)
            {
                res.LocationLatitude = item.Location.Latitude;
                res.Locationlongitude = item.Location.Longitude;
                res.LocationAltitute = item.Location.Altitude;
            }
            if(item.Thumbnails != null && item.Thumbnails.CurrentPage != null && item.Thumbnails.CurrentPage.Any())
            {
                var set = item.Thumbnails.CurrentPage.First();
                res.ThumbSmall = set.Small.Url;
                res.ThumbMid = set.Medium.Url;
                res.ThumbLarge = set.Large.Url;
            }
            if (item.CreatedBy != null && item.CreatedBy.User != null) res.CreatedByUser = item.CreatedBy.User.DisplayName;
            if (item.CreatedBy != null && item.CreatedBy.Application != null) res.CreatedByApp = item.CreatedBy.Application.DisplayName;

            return res;
        }
    }
}
