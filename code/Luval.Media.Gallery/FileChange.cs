using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    public enum FileChange
    {
        /// <summary>
        /// File changed
        /// </summary>
        Changed, 
        /// <summary>
        /// File created
        /// </summary>
        Created, 
        /// <summary>
        /// File deleted
        /// </summary>
        Deleted, 
        /// <summary>
        /// File renamed
        /// </summary>
        Rename
    }
}
