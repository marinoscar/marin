using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Workflow
{
    public class Runner
    {
        private readonly IList<IActivity> _activities = new List<IActivity>();
        private readonly ISessionStore _store;
        private readonly ILogger _logger;

        public Runner(IEnumerable<IActivity> activities, ISessionStore sessionStore, ILogger logger)
        {
            _store = sessionStore;
            _activities = new List<IActivity>(activities);
            _logger = logger;
        }

        public async Task StartSession()
        {
            var tokenSource = new CancellationTokenSource();
            var context = new SessionContext() { Logger = _logger, RunnerType = GetType().FullName };
            context.SetStart();
            LogInformation("Started session {0}", context.Id);
            await _store.CreateSessionAsync(context);
            try
            {
                foreach (var activity in _activities)
                {
                    await RunActivity(context, activity, tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                context.SetException(ex);
                await _store.UpdateSessionAsync(context);
            }
            context.SetComplete();
            await _store.UpdateSessionAsync(context);
            LogInformation("Completed session {0} {1} activities ran", context.Id, context.ActivityExecutions.Count);
        }

        protected virtual async Task RunActivity(SessionContext context, IActivity activity, CancellationToken cancellationToken)
        {
            var stats = CreateStats(context, activity);
            context.ActivityExecutions.Add(stats);
            stats.SetStart();
            await _store.CreateActivityStatusAsync(stats);
            try
            {
                LogInformation("{0} started running", activity.Name.DisplayName);
                await activity.ExecuteAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                stats.SetException(ex);
                LogError("{0} failed with error: {1}", activity.Name.DisplayName, ex);
                _store.UpdateActivityStatusAsync(stats);
                return;
            }
            stats.SetComplete();
            _store.UpdateActivityStatusAsync(stats);
            LogInformation("{0} completed", activity.Name.DisplayName);
            return;
        }

        private static ActivityExecutionStatus CreateStats(SessionContext context, IActivity activity)
        {
            var stats = new ActivityExecutionStatus()
            {
                ActivitySessionId = context.Id,
                ActivityName = activity.Name.DisplayName,
                ActivityType = activity.GetType().FullName,
            };
            return stats;
        }

        protected virtual Task LogInformation(string message, params object[] args)
        {
            if (_logger == null) return Task.CompletedTask;
            return Task.Run(() => _logger.LogInformation(message, args));
        }

        protected virtual Task LogWarning(string message, params object[] args)
        {
            if (_logger == null) return Task.CompletedTask;
            return Task.Run(() => _logger.LogWarning(message, args));
        }

        protected virtual Task LogError(string message, params object[] args)
        {
            if (_logger == null) return Task.CompletedTask;
            return Task.Run(() => _logger.LogError (message, args));
        }
    }
}
