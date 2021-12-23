using Luval.Common.Security;
using Luval.Data.Sql;
using Luval.Media.Gallery;
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
            var uowFactory = new SqlServerUnitOfWorkFactory("Server=.\\SQLEXPRESS;Database=MarinDb;Trusted_Connection=True;");
            var safeItemRepo = new SafeItemRepository(uowFactory.Create<SafeItem, string>());
            var authenticationOpts = new SafeItemAuthenticationProvider(safeItemRepo, "oscar.marin.saenz@outlook.com");
            SafeString.SetKeyString("782F413F442A472D4B6150645367566B");
            var options = FromConfig();
            var mediaProvider = new MediaDriveProvider(authenticationOpts);
            var t = mediaProvider.GetMediaItemsFromDriveItemAsync("7182B0080429DBE3!66401", true, CancellationToken.None);
            t.Wait();
            var items = t.Result;
            var repo = new MediaGalleryRepository(uowFactory.Create<MediaItem, string>());
            var tasks = new List<Task>();
            foreach (var item in items)
            {
                var insertT = repo.CreateAsync(item, CancellationToken.None);
                tasks.Add(insertT);
                insertT.Wait();
            }
            Task.WaitAll(tasks.ToArray());
        }


    }
}
