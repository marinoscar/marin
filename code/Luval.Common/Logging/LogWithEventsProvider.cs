using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Logging
{
    public class LogWithEventsProvider : ILoggerProvider
    {

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new LogWithEvents(name));

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
