using System.Threading;
using System.Threading.Tasks;

namespace Luval.Common.Logging
{
    public interface ILoggingRepository
    {
        void WriteLogMessage(LogMessage message);
        Task WriteLogMessageAsync(LogMessage message);
        Task WriteLogMessageAsync(LogMessage message, CancellationToken cancellationToken);
    }
}