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
    public abstract class BaseImageHashProvider : IImageHashProvider
    {
        /// <inheritdoc/>
        public ulong FromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Invalid file path provided", fileName);
            using (var fs  = File.OpenRead(fileName))
            {
                return FromStream(fs);
            }
        }

        /// <inheritdoc/>
        public ulong FromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            var hashAlgorithm = GetProvider();
            return hashAlgorithm.Hash(stream);
        }

        protected abstract IImageHash GetProvider();
    }
}
