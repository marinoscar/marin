using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Logging
{
    public abstract class LoggerBase : ILogger
    {
        protected virtual string Name { get; private set; }
        protected virtual IExternalScopeProvider ScopeProvider { get; private set; }
        protected virtual Func<string, LogLevel, bool> Filter { get; private set; }

        public LoggerBase(string name, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider)
        {
            Name = name; ScopeProvider = scopeProvider; Filter = filter;
        }

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            if (Filter == null) return true;
            if (logLevel == LogLevel.None)
                return false;
            return Filter(Name, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                LogMessage(logLevel, eventId, message, exception);
            }
        }

        protected abstract void LogMessage(LogLevel logLevel, EventId eventId, string message, Exception exception);

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? EmptyScope.Instance;
    }

    public class EmptyScope : IExternalScopeProvider, IDisposable
    {
        public static EmptyScope Instance { get; } = new EmptyScope();

        private EmptyScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        public IDisposable Push(object state)
        {
            return null;
        }
    }
}
