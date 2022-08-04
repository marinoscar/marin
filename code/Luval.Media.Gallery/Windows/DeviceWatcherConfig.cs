using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Windows
{
    /// <summary>
    /// Configuration settings for the watcher
    /// </summary>
    public class DeviceWatcherConfig
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public DeviceWatcherConfig()
        {
            Folders = new List<string>();
            SupportedExtensions = new List<string>();
            IncludeSubdirectories = true;
        }

        /// <summary>
        /// Gets a list with the folers that would be monitoring
        /// </summary>
        public List<string> Folders { get; private set; }

        /// <summary>
        /// Gets a list of the supported extensions, if empty then all are supported, otherwise only the ones in the list
        /// </summary>
        /// <example>".txt", ".png", etc.</example>
        public List<string> SupportedExtensions { get; private set; }
        /// <summary>
        /// Indicates if will be monitoring subdirectories
        /// </summary>
        public bool IncludeSubdirectories { get; set; }
    }
}
