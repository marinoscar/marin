using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marin.Web.Workers
{
    public abstract class TimeWorkerAsync : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly TimerWorkerOptions _options;
        private Timer _timer = null!;
        private readonly TimeSpan _dueTime;

        public TimeWorkerAsync(TimerWorkerOptions options)
        {
            _options = options;
            if (options.StartTime != null && options.StartTime.Value.UtcDateTime > DateTime.UtcNow)
                _dueTime = options.StartTime.Value.UtcDateTime.Subtract(DateTime.UtcNow);
            else _dueTime = TimeSpan.Zero;
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            //Initialize the timer
            _timer = new Timer(DoWork, _stoppingCts, _dueTime, _options.Interval);

            // Otherwise it's running
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called everytime the timer is called
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        protected virtual void DoWork(object? state)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}
