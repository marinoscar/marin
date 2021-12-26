using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Logging
{
    public class LogWithEvents : LoggerBase
    {

        public LogWithEvents(string loggerName) : this(loggerName, ((c, l) => true), EmptyScope.Instance)
        {

        }
        public LogWithEvents(string loggerName, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider) : base(loggerName, filter, scopeProvider)
        {

        }

        protected override void LogMessage(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            OnMessageLogged(new LogEventArgs(Luval.Common.Logging.LogMessage.Create(nameof(LogWithEvents),
                logLevel, eventId, exception, message)));
        }

        public event EventHandler<LogEventArgs> MessageLogged;

        protected virtual void OnMessageLogged(LogEventArgs e)
        {
            MessageLogged?.Invoke(this, e);
        }

    }

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }

        public LogEventArgs(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            LogMessage = LogMessage.Create(loggerName, logLevel, eventId, exception, message);
        }

        public LogMessage LogMessage { get; private set; }
    }
}
