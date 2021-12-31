using Luval.Common;
using Luval.Common.Security;
using Luval.Data.Extensions;
using Luval.Data.Sql;
using Luval.Media.Gallery;
using Luval.Media.Gallery.Entities;
using Luval.Media.Gallery.OneDrive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var start = DateTime.Now;
            Console.WriteLine("Started at {0}", start);
            var uowFactory = new SqlServerUnitOfWorkFactory("Server=.\\SQLEXPRESS;Database=MarinDb;Trusted_Connection=True;");
            var graphTokenRepo = new GraphAutenticationRepository(uowFactory.Create<GraphAuthenticationToken, string>());
            var authenticationOpts = new GraphTokenAuthenticationStore(graphTokenRepo, "oscar.marin.saenz@outlook.com");
            SafeString.SetKeyString("782F413F442A472D4B6150645367566B");
            var options = FromConfig();
            var mediaProvider = new MediaDriveProvider(authenticationOpts);
            mediaProvider.FolderItemProcessed += MediaProvider_FolderItemProcessed;
            var t = mediaProvider.GetMediaItemsFromDriveItemAsync("7182B0080429DBE3!315", true, CancellationToken.None);
            t.Wait();
            var items = t.Result;
            var repo = new MediaGalleryRepository(uowFactory.Create<MediaItem, string>());
            var tasks = new List<Task>();
            var res = 0;
            Console.WriteLine("Saving to file");
            SaveAsCSV(items);
            foreach (var item in items)
            {
                Console.WriteLine("[{0}] - Persisting {1}", DateTime.Now.TimeOfDay, item.Name);
                res += repo.CreateAsync(item, CancellationToken.None).Result;
            }
            Console.WriteLine("Completed at {0} total duration {1}", DateTime.Now, DateTime.Now.Subtract(start).ToString("HH:mm:ss"));
        }

        private void MediaProvider_FolderItemProcessed(object sender, DriveItemEventArgs e)
        {
            Console.WriteLine("[{0}] - Working with Folder {1} With {2} items", DateTime.Now.TimeOfDay, e.DriveItem.Name, e.DriveItem.Folder.ChildCount);
        }

        private void SaveAsCSV(IEnumerable<object> items)
        {
            if (items == null || !items.Any()) return;
            var first = items.First();
            var props = first.GetType().GetProperties();
            using (var sw = new StringWriter())
            {
                sw.WriteLine(string.Join(",", props.Select(i => string.Format(@"""{0}""", i.Name))));
                foreach (var item in items)
                {
                    sw.WriteLine(string.Join(",", props.Select(i => string.Format(@"""{0}""", i.GetValue(item)))));
                }
                File.WriteAllText(@"C:\Users\CH489GT\Downloads\output.csv", sw.ToString());
            }
        }
    }
}
