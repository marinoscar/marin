using Luval.Data.Sql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Logging
{
    public class SqlLogger : LoggingStore
    {
        public SqlLogger(string connectionString) : this(connectionString, (name, level) => { return true; }, EmptyScope.Instance)
        {

        }

        public SqlLogger(string connectionString, IExternalScopeProvider scopeProvider) : this(connectionString, (name, level) => { return true; }, scopeProvider)
        {

        }

        public SqlLogger(string connectionString, Func<string, LogLevel, bool> filter) : this(connectionString, filter, EmptyScope.Instance)
        {
        }

        public SqlLogger(string connectionString, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider) : this(connectionString, nameof(SqlLogger), filter, scopeProvider)
        {
        }

        public SqlLogger(string connectionString, string loggerName, Func<string, LogLevel, bool> filter, IExternalScopeProvider scopeProvider) : base(CreateRepo(connectionString), loggerName, filter, scopeProvider)
        {
        }

        private static ILoggingRepository CreateRepo(string connStr)
        {
            var factory = new SqlServerUnitOfWorkFactory(connStr);
            return new LoggingRepository(factory.Create<LogMessage, long>());
        }
    }
}
