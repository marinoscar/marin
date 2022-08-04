using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    /// <summary>
    /// Provides the arguments for the file change
    /// </summary>
    public class FileChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Type of change for the file
        /// </summary>
        public FileChange Change { get; set; }
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; }  
    }
}
