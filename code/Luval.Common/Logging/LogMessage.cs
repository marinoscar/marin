using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Luval.Common.Logging
{
    public class LogMessage : IIdBasedEntity<long>
    {
        public LogMessage()
        {
            MachineName = Environment.MachineName;
            UtcTimestamp = DateTime.UtcNow;
        }

        [PrimaryKey, IdentityColumn]
        public long Id { get; set; }
        public string MachineName { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public int MessageType { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        public static LogMessage Create(string loggerName, LogLevel logLevel, EventId eventId, Exception exception, string message)
        {
            return new LogMessage()
            {
                Message = message,
                Exception = exception?.ToString() ?? null,
                Logger = loggerName,
                MachineName = Environment.MachineName,
                MessageType = Convert.ToInt32(logLevel),
                UtcTimestamp = DateTime.UtcNow
            };
        }
    }
}
