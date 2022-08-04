using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Windows
{
    /// <summary>
    /// Provides an implementation to detect changes on a MS Windows device
    /// </summary>
    public class DeviceWatcher : IDeviceFileChangesWatcher
    {
        private readonly ILogger _logger;
        private DeviceWatcherConfig _config;
        private Dictionary<DirectoryInfo, FileSystemWatcher> _watchers;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="watcherConfig">Configuration settings</param>
        /// <param name="logger">Instance to log for errors and events</param>
        public DeviceWatcher(DeviceWatcherConfig watcherConfig, ILogger logger)
        {
            _config = watcherConfig ?? throw new ArgumentNullException("watcherConfig");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        /// <inheritdoc/>
        public event EventHandler<FileChangeEventArgs> ChangeDetected;

        /// <inheritdoc/>
        public void Start()
        {
            _watchers = new Dictionary<DirectoryInfo, FileSystemWatcher>();
            foreach (var directory in _config.Folders)
            {
                var dir = new DirectoryInfo(directory);
                if (!dir.Exists)
                {
                    _logger.LogWarning("Folder {0} does not exist", dir.FullName);
                    continue;
                }
                if (_watchers.ContainsKey(dir)) continue;
                _watchers[dir] = new FileSystemWatcher(dir.FullName);
                _watchers[dir].Filter = _config.Filter;
                _watchers[dir].EnableRaisingEvents = true;
                _watchers[dir].IncludeSubdirectories = _config.IncludeSubdirectories;
                _watchers[dir].NotifyFilter =
                    NotifyFilters.FileName | NotifyFilters.DirectoryName |
                    NotifyFilters.CreationTime | NotifyFilters.Size |
                    NotifyFilters.FileName | NotifyFilters.LastWrite;

                _watchers[dir].Changed += OnEventDetected;
                _watchers[dir].Created += OnEventDetected;
                _watchers[dir].Renamed += OnEventDetected;
                _watchers[dir].Deleted += OnEventDetected;
                _watchers[dir].Error += OnErrorDetected;
            }
        }

        private void OnErrorDetected(object sender, ErrorEventArgs e)
        {
            _logger.LogError("Monitoring error: {0}", e.GetException().Message);
        }

        private void OnEventDetected(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Event detected: {0} on file {1}", e.ChangeType, e.FullPath);
            ChangeDetected?.Invoke(this, new FileChangeEventArgs()
            {
                FileName = e.FullPath, Change = MapValue(e.ChangeType)
            });
        }

        /// <inheritdoc/>
        public void Stop()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.Dispose();
            }
            _watchers = null;
        }

        private FileChange MapValue(WatcherChangeTypes type)
        {
            switch (type)
            {
                case WatcherChangeTypes.Created:
                    return FileChange.Created;
                case WatcherChangeTypes.Deleted:
                    return FileChange.Deleted;
                case WatcherChangeTypes.Changed:
                    return FileChange.Changed;
                case WatcherChangeTypes.Renamed:
                    return FileChange.Rename;
                case WatcherChangeTypes.All:
                    return FileChange.Created;
                default:
                    return FileChange.Created;
            }
        }
    }
}
