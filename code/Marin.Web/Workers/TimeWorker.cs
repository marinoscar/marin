using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marin.Web.Workers
{
    public abstract class TimeWorker : TimeWorkerAsync
    {
        public TimeWorker(TimerWorkerOptions options) : base(options)
        {

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return Task.CompletedTask;
            return Task.Run(() =>
            {
                Execute();
            });
        }

        public abstract void Execute();
    }
}
