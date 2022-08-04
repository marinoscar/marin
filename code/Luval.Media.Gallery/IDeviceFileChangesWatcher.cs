using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    /// <summary>
    /// Provides an abstraction to the detect file changes on a device
    /// </summary>
    public interface IDeviceFileChangesWatcher
    {
        /// <summary>
        /// Starts monitoring changes
        /// </summary>
        void Start();
        /// <summary>
        /// Stops monitoring changes
        /// </summary>
        void Stop();
        /// <summary>
        /// Identifies when a change takes place
        /// </summary>
        event EventHandler<FileChangeEventArgs> ChangeDetected;
    }
}
