using Luval.Common.Security;
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
    public class SafeItemAuthenticationProvider : IAuthenticationProvider
    {
        ISafeItemRepository _itemRepository;
        string _accountPrincipalName;

        public SafeItemAuthenticationProvider(ISafeItemRepository itemRepository, string accountPrincipalName)
        {
            _itemRepository = itemRepository;
            _accountPrincipalName = accountPrincipalName;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await GetToken();
            request.Headers.Add("Authorization", string.Format("Bearer {0}", token.ItemValue));
        }

        private async Task<SafeItem> GetToken()
        {
            var item = await _itemRepository.GetByNameAsync(string.Format("{0}:Token", _accountPrincipalName), CancellationToken.None);
            if (item == null) throw new ArgumentException("No token on record for account principal: {0}", _accountPrincipalName);
            return item;
        }
    }
}
