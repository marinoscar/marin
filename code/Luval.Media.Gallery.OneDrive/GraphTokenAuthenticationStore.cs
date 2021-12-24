using Luval.Common.Security;
using Luval.Media.Gallery.Entities;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class GraphTokenAuthenticationStore : IAuthenticationProvider
    {
        protected IGraphAutenticationRepository Repository { get; private set; }
        protected string AccountPrincipalEmail { get; private set; }

        public GraphTokenAuthenticationStore(IGraphAutenticationRepository grapTokenRepo, string accountPrincipalEmail)
        {
            Repository = grapTokenRepo;
            AccountPrincipalEmail = accountPrincipalEmail;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Add("Authorization", string.Format("Bearer {0}", token.Token));
        }

        private async Task<GraphAuthenticationToken> GetToken()
        {
            return await Repository.GetByUserEmailAsync(AccountPrincipalEmail, CancellationToken.None);
        }
    }
}
