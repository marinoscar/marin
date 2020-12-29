using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marin
{
    public class ConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    Console.WriteLine("[{0}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logLevel, Convert.ToString(state));
                    break;
                case LogLevel.Error:
                    Console.WriteLine("[{0}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logLevel, Convert.ToString(state));
                    break;
                case LogLevel.Critical:
                    Console.WriteLine("[{0}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logLevel, Convert.ToString(state));
                    break;
                default:
                    Console.WriteLine("[{0}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logLevel, Convert.ToString(state));
                    break;
            }
        }
    }
}
