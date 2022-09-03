using Luval.DataStore.Extensions;
using Luval.FileSync.Core.Entities;
using Luval.FileSync.Core.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Media
{
    public class MediaReader
    {
        public static LocalMediaFile FromFile(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (!fileInfo.Exists) throw new ArgumentException("File does not exists", nameof(fileInfo));
            var result = new LocalMediaFile() { 
                LocationInDevice = fileInfo.FullName,
                UtcFileCreatedOn = fileInfo.CreationTimeUtc.ToElapsedSeconds(),
                UtcFileModifiedOn = fileInfo.LastWriteTimeUtc.ToElapsedSeconds(),
            };
            using (var fs = fileInfo.OpenRead())
            {
                result.ImageHash = Convert.ToString(HashProvider.FromStream(fs));
                result.Hash = HashProvider.MD5FromStream(fs);
            }
            return result;
        }

    }
}
