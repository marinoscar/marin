using Luval.Media.Gallery.Entities;
using Luval.Media.Gallery.OneDrive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marin.Sink.Tests
{
    public class Gallery
    {
        private AuthenticationOptions FromConfig()
        {
            return JsonConvert.DeserializeObject<AuthenticationOptions>(File.ReadAllText("onedriveconfig.json"));
        }

        public void DoTest()
        {
            var options = FromConfig();
            var mediaProvider = new MediaDriveProvider(options);
            var t = mediaProvider.GetItemsFromDriveAsync(new MediaDrive() {
                DriveId = "7182b0080429dbe3",
                LookInChildren = true,
                DrivePath = "root",
                Provider = "OneDrive"
            }, CancellationToken.None);
            t.Wait();
            var items = t.Result;
        }


    }
}
