using Luval.Data.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Logging
{
    public class LoggingStore : LoggerBase
    {

        protected virtual ILoggingRepository LoggingRepository { get; private set; }

        public LoggingStore(ILoggingRepository loggingRepository, string name, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider) : base(name, filter, scopeProvider)
        {
            LoggingRepository = loggingRepository;
        }

        protected override void LogMessage(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            LogMessage(Luval.Common.Logging.LogMessage.Create(Name, logLevel, eventId, exception, message));
        }

        public void LogMessage(LogMessage logMessage)
        {
            LoggingRepository.WriteLogMessageAsync(logMessage);
        }
    }
}
