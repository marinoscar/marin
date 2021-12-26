using Luval.Common.Logging;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marin.Web.Workers
{
    public class SampleWorker : TimeWorker
    {
        private int _executionCount = 60;
        protected virtual SqlLogger SqlLogger { get; private set; }
        protected virtual LogWithEvents Events { get; set; }
        private readonly ConcurrentStack<LogMessage> _messages;

        public SampleWorker(LogWithEvents logWithEvents, SqlLogger logger) : base(new TimerWorkerOptions() { Interval = TimeSpan.FromMinutes(2) })
        {
            SqlLogger = logger;
            Events = logWithEvents;
            Events.MessageLogged += Events_MessageLogged;
            _messages = new ConcurrentStack<LogMessage>();
        }

        private void Events_MessageLogged(object sender, LogEventArgs e)
        {
            _messages.Push(e.LogMessage);
        }

        public override void Execute()
        {
            var count = 0;
            while (_messages.Count > 0 || count < _executionCount)
            {
                LogMessage m = null;
                if (_messages.TryPop(out m))
                {
                    SqlLogger.LogMessage(m);
                }
                count++;
            }
        }
    }
}
