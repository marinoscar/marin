using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Workflow
{
    public interface ISessionStore
    {
        Task CreateSessionAsync(SessionContext session);
        Task UpdateSessionAsync(SessionContext session);

        Task CreateActivityStatusAsync(ActivityExecutionStatus executionStatus);
        Task UpdateActivityStatusAsync(ActivityExecutionStatus executionStatus);

    }
}
