using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class TokenAuthenticatorProvider : IAuthenticationProvider
    {
        private string _token;
        public TokenAuthenticatorProvider(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");
            _token = token;
        }

        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            return Task.Run(() => {
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _token));
            });
        }
    }
}
