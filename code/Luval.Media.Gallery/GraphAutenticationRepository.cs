using Luval.Common.Security;
using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    public class GraphAutenticationRepository
    {
        protected virtual IUnitOfWork<GraphAuthenticationToken, string> UnitOfWork { get; private set; }

        public async Task<int> CreateOrUpdateAsync(GraphTokenResponse tokenResponse, GraphPrincipal principal, CancellationToken cancellationToken)
        {
            var item = new GraphAuthenticationToken()
            {
                UtcExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                PrincipalEmail = principal.UserPrincipalName,
                UserId = principal.Id,
                Token = SafeString.EncryptString(tokenResponse.AccessToken),
                RefreshToken = SafeString.EncryptString(tokenResponse.RefreshToken),
                IdToken = SafeString.EncryptString(tokenResponse.IdToken)
            };
            var tokens = await UnitOfWork.Entities.Query.GetAsync(i => i.UserId == principal.Id, cancellationToken);
            if (tokens == null || !tokens.Any()) return await UnitOfWork.AddAndSaveAsync(item, cancellationToken);
            else
            {
                item.Id = tokens.First().Id;
                return await UnitOfWork.UpdateAndSaveAsync(item, cancellationToken);
            }
        }
    }
}
