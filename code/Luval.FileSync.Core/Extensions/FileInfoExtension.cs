using Luval.FileSync.Core.Entities;
using Luval.FileSync.Core.Hash;
using Luval.FileSync.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Extensions
{
    public static class FileInfoExtension
    {
        /// <summary>
        /// Creates an instance of <see cref="MediaFile"/> from the information in the <see cref="FileInfo"/> object
        /// </summary>
        /// <param name="f">The file information</param>
        /// <returns>An instance of <see cref="MediaFile"/></returns>
        public static MediaFile ToMediaFile(this FileInfo f)
        {
            var res = new MediaFile() { 
                UtcFileCreatedOn = f.CreationTimeUtc, UtcFileModifiedOn = f.LastWriteTimeUtc, Format = GetFormat(f)
            };
            if (f.IsImageFile())
            {
                using (var s = f.OpenRead())
                {
                    var hash = HashProvider.FromStream(s, true);
                    var md5 = HashProvider.MD5FromStream(s);
                    var meta = ImageMetadataReader.FromStream(s);
                    res.ImageHash = hash.ToString();
                    res.Hash = md5;
                    res.UtcImageTakenOn = meta.UtcDateTaken;
                    res.Latitude = meta.GeoLocation.Latitude;
                    res.Longitude = meta.GeoLocation.Longitude;
                    res.Altitude = meta.GeoLocation.Altitude;
                }
            }
            else res.Hash = HashProvider.MD5FromFile(f.FullName);
            return res;
        }

        /// <summary>
        /// Indicates if the file is an image
        /// </summary>
        public static bool IsImageFile(this FileInfo f)
        {
            return new[] { ".png", ".bmp", ".gif", ".jpg", ".jpeg", ".gif" }.Contains(f.Extension); 
        }

        /// <summary>
        /// Indicates if the fule is a video format
        /// </summary>
        public static bool IsVideoFile(this FileInfo f)
        {
            return new[] { ".avi", ".wmv", ".mov", ".mp4", ".flv", ".mkv" }.Contains(f.Extension);
           
        }

        /// <summary>
        /// Indicates of the file is a media file (video or image)
        /// </summary>
        public static bool IsMediaFile(this FileInfo f)
        {
            return f.IsImageFile() || f.IsVideoFile();
        }

        private static string GetFormat(this FileInfo f)
        {
            if (f.IsImageFile()) return "Image";
            if (f.IsVideoFile()) return "Video";
            return "File";
        }
    }
}
