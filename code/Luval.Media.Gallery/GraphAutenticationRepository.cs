using Luval.Common.Security;
using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    public class GraphAutenticationRepository : IGraphAutenticationRepository
    {
        protected virtual IUnitOfWork<GraphAuthenticationToken, string> UnitOfWork { get; private set; }

        public GraphAutenticationRepository(IUnitOfWork<GraphAuthenticationToken, string> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<int> CreateOrUpdateAsync(GraphTokenResponse tokenResponse, GraphPrincipal principal, string userId, CancellationToken cancellationToken)
        {
            var item = new GraphAuthenticationToken()
            {
                UtcExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                PrincipalEmail = principal.UserPrincipalName,
                UserId = principal.Id,
                Token = SafeString.EncryptString(tokenResponse.AccessToken),
                RefreshToken = SafeString.EncryptString(tokenResponse.RefreshToken),
                IdToken = SafeString.EncryptString(tokenResponse.IdToken),
                UpdatedByUserId = userId,
                CreatedByUserId = userId
                
            };
            var tokens = await UnitOfWork.Entities.Query.GetAsync(i => i.UserId == principal.Id, cancellationToken);
            if (tokens == null || !tokens.Any()) return await UnitOfWork.AddAndSaveAsync(item, cancellationToken);
            else
            {
                item.Id = tokens.First().Id;
                item.UpdatedByUserId = userId;
                item.UtcUpdatedOn = DateTime.UtcNow;
                return await UnitOfWork.UpdateAndSaveAsync(item, cancellationToken);
            }
        }

        public Task<GraphAuthenticationToken> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return GetAsync(i => i.UserId == userId, cancellationToken);
        }

        public Task<GraphAuthenticationToken> GetByUserEmailAsync(string email, CancellationToken cancellationToken)
        {
            return GetAsync(i => i.PrincipalEmail == email, cancellationToken);
        }

        private async Task<GraphAuthenticationToken> GetAsync(Expression<Func<GraphAuthenticationToken, bool>> whereExpression, CancellationToken cancellationToken)
        {
            var res = (await UnitOfWork.Entities.Query.GetAsync(whereExpression, cancellationToken)).FirstOrDefault();
            res.Token = SafeString.DecryptString(res.Token);
            res.RefreshToken = SafeString.DecryptString(res.RefreshToken);
            res.IdToken = SafeString.DecryptString(res.IdToken);
            return res;
        }



    }
}
