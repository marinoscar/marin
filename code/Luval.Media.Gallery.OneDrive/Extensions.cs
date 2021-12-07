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
                res.MediaType = item.File.MimeType;
                res.Media256Hash = item.File.Hashes.Sha256Hash;
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
            if (item.CreatedByUser != null)
                res.CreatedByUser = item.CreatedByUser.DisplayName;
            if (item.CreatedBy != null) res.CreatedByApp = item.CreatedBy.Application.DisplayName;

            return res;
        }
    }
}
