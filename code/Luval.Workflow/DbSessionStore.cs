using Luval.Data;
using Luval.Data.Interfaces;
using Luval.Data.Sql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Workflow
{
    public class DbSessionStore : ISessionStore
    {
        protected Database Database { get; private set; }
        protected IUnitOfWork<SessionContext, string> SessionAdapter { get; set; }

        protected IUnitOfWork<ActivityExecutionStatus, string> ActivityAdapter { get; set; }

        public DbSessionStore(string sqlConnectionString)
        {
            Database = new SqlServerDatabase(sqlConnectionString);
            var uowFactory = new DbUnitOfWorkFactory(Database, new SqlServerDialectFactory());
            SessionAdapter = uowFactory.Create<SessionContext, string>();
            ActivityAdapter = uowFactory.Create<ActivityExecutionStatus, string>();
        }

        public Task CreateActivityStatusAsync(ActivityExecutionStatus executionStatus)
        {
            ActivityAdapter.Entities.Add(executionStatus);
            return ActivityAdapter.SaveChangesAsync(CancellationToken.None);
        }

        public Task CreateSessionAsync(SessionContext session)
        {
            SessionAdapter.Entities.Add(session);
            return SessionAdapter.SaveChangesAsync(CancellationToken.None);
        }

        public Task UpdateActivityStatusAsync(ActivityExecutionStatus executionStatus)
        {
            ActivityAdapter.Entities.Update(executionStatus);
            return ActivityAdapter.SaveChangesAsync(CancellationToken.None);
        }

        public Task UpdateSessionAsync(SessionContext session)
        {
            SessionAdapter.Entities.Update(session);
            return SessionAdapter.SaveChangesAsync(CancellationToken.None);
        }
    }
}
