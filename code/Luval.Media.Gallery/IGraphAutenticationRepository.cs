using Luval.Media.Gallery.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    public interface IGraphAutenticationRepository
    {
        Task<int> CreateOrUpdateAsync(GraphTokenResponse tokenResponse, GraphPrincipal principal, string userId, CancellationToken cancellationToken);
        Task<GraphAuthenticationToken> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<GraphAuthenticationToken> GetByUserEmailAsync(string email, CancellationToken cancellationToken);
    }
}