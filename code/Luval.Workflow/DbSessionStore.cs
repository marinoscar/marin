﻿using Luval.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Workflow
{
    public class DbSessionStore : ISessionStore
    {
        protected Database Database { get; private set; }
        protected EntityAdapter<SessionContext> SessionAdapter { get; set; }

        protected EntityAdapter<ActivityExecutionStatus> ActivityAdapter { get; set; }

        public DbSessionStore(string sqlConnectionString)
        {
            Database = new SqlServerDatabase(sqlConnectionString);
            SessionAdapter = new EntityAdapter<SessionContext>(Database, new SqlServerDialectFactory());
            ActivityAdapter = new EntityAdapter<ActivityExecutionStatus>(Database, new SqlServerDialectFactory());
        }

        public Task CreateActivityStatusAsync(ActivityExecutionStatus executionStatus)
        {
            return ActivityAdapter.InsertAsync(executionStatus);
        }

        public Task CreateSessionAsync(SessionContext session)
        {
            return SessionAdapter.InsertAsync(session);
        }

        public Task UpdateActivityStatusAsync(ActivityExecutionStatus executionStatus)
        {
            return ActivityAdapter.UpdateAsync(executionStatus);
        }

        public Task UpdateSessionAsync(SessionContext session)
        {
            return SessionAdapter.UpdateAsync(session);
        }
    }
}
