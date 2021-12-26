using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Common.Logging
{
    public class LoggingRepository : ILoggingRepository
    {
        public LoggingRepository(IUnitOfWork<LogMessage, long> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected virtual IUnitOfWork<LogMessage, long> UnitOfWork { get; private set; }


        public Task WriteLogMessageAsync(LogMessage message, CancellationToken cancellationToken)
        {
            return UnitOfWork.AddAndSaveAsync(message, cancellationToken);
        }

        public Task WriteLogMessageAsync(LogMessage message)
        {
            return WriteLogMessageAsync(message, CancellationToken.None);
        }

        public void WriteLogMessage(LogMessage message)
        {
            UnitOfWork.AddAndSave(message);
        }

    }
}
