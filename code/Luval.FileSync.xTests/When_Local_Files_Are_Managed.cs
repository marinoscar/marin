using Luval.FileSync.Core.Data;
using Luval.FileSync.Core.Data.Store;
using Luval.FileSync.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.xTests
{
    public class When_Local_Files_Are_Managed
    {
        [Fact]
        public static void It_Should_Add_File_To_Local_Database()
        {
            var file = new FileInfo(LocalDb.GetLocation());
            if(file.Exists) file.Delete();

            var repo = new LocalRepository();
            var copy1 = Environment.CurrentDirectory + @"\resources\images\object.jpg";
            var copyFile1 = new FileInfo(copy1);
            var mediaItem1 = copyFile1.ToMediaFile();

            var first = repo.AddItem(mediaItem1);
            var second = repo.AddItem(mediaItem1);

            Assert.Equal(1, first);
            Assert.Equal(0, second);

        }
    }
}
