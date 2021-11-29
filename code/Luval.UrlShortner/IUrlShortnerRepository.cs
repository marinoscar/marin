using System.Threading;
using System.Threading.Tasks;

namespace Luval.UrlShortner
{
    public interface IUrlShortnerRepository
    {
        Task<ShortName> AddUrlAsync(string uri, CancellationToken cancellationToken);
        Task<ShortName> AddUrlAsync(string id, string uri, CancellationToken cancellationToken);
        Task<ShortName> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<int> UpdateAsync(ShortName item, CancellationToken cancellationToken);
        Task<int> UpdateAsync(string id, CancellationToken cancellationToken);
    }
}