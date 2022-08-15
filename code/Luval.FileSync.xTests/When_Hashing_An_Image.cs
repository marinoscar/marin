using Luval.FileSync.Core.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.xTests
{
    public class When_Hashing_An_Image
    {
        [Fact]
        public void It_Should_Identify_Duplicates()
        {
            var copy1 = Environment.CurrentDirectory + @"\resources\images\object.jpg";
            var copy2 = Environment.CurrentDirectory + @"\resources\images\object-smaller.jpg";
            var copy3 = Environment.CurrentDirectory + @"\resources\images\object-wide.jpg";
            var copy4 = Environment.CurrentDirectory + @"\resources\images\object-filter.jpg";

            HashProvider.EdgeDetector(copy1);
            HashProvider.EdgeDetector(copy4);

            var hash1 = HashProvider.FromFile(copy1);
            var hash2 = HashProvider.FromFile(copy2);
            var hash3 = HashProvider.FromFile(copy3);

            Assert.Equal(hash1.PerceptualHash, hash2.PerceptualHash);
        }
    }
}
