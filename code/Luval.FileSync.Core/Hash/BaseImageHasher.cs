using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

        public ulong FromImage(Image<Rgba32> image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            var hashAlgorithm = GetProvider();
            return hashAlgorithm.Hash(image);
        }

        protected abstract IImageHash GetProvider();
    }
}
