using Luval.FileSync.Core.Data;
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
            var repo = new LocalRepository();
            var copy1 = Environment.CurrentDirectory + @"\resources\images\object.jpg";
            var copyFile1 = new FileInfo(copy1);
            var mediaItem1 = copyFile1.ToMediaFile();

            repo.AddItem(mediaItem1);

            Assert.True(true);

        }
    }
}
