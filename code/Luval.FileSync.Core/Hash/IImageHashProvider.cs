using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Hash
{
    /// <summary>
    /// Creates a hash for an image
    /// </summary>
    public interface IImageHashProvider
    {
        /// <summary>
        /// Creates the hash from the file name
        /// </summary>
        /// <param name="fileName">The name of the file to hash</param>
        /// <returns>The image file hash</returns>
        ulong FromFile(string fileName);
        /// <summary>
        /// Creates the hash from the file name
        /// </summary>
        /// <param name="stream">The stream for the image to hash</param>
        /// <returns>The image file hash</returns>
        ulong FromStream(Stream stream);
    }
}
