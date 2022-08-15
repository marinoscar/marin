using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Hash
{
    /// <inheritdoc/>
    public class DifferenceHashProvider : BaseImageHashProvider
    {
        protected override IImageHash GetProvider()
        {
            return new DifferenceHash();
        }
    }
}
