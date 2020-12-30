using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Workflow
{
    public interface IActivity
    {
        /// <summary>
        /// Gets the name information for the activity
        /// </summary>
        IActivityName Name { get; }

        /// <summary>
        /// Runs the activity
        /// </summary>
        /// <param name="context">The session context</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        Task ExecuteAsync(SessionContext context, CancellationToken cancellationToken);
    }
}