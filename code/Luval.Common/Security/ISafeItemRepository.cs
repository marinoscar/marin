using System.Threading;
using System.Threading.Tasks;

namespace Luval.Common.Security
{
    public interface ISafeItemRepository
    {
        Task<int> AddOrUpdateAsync(string name, string unEncryptedValue, CancellationToken cancellationToken);
        Task<int> AddOrUpdateAsync(string name, string unEncryptedValue, string userId, CancellationToken cancellationToken);
        Task<int> DeleteAsync(SafeItem item, CancellationToken cancellationToken);
        Task<int> DeleteByIdAsync(string id, CancellationToken cancellationToken);
        Task<int> DeleteByNameAsync(string itemName, CancellationToken cancellationToken);
        Task<SafeItem> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<SafeItem> GetByNameAsync(string itemName, CancellationToken cancellationToken);
    }
}