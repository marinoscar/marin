using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class GraphClient
    {

        private AuthenticationResult _authenticationResult;
        private HttpClient _httpClient;

        public GraphClient(AuthenticationResult authenticationResult)
        {
            _authenticationResult = authenticationResult;
            _httpClient = new HttpClient();
        }

        public virtual async Task<HttpResponseMessage> RunGraphRequestAsync(string requestUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_authenticationResult.AccessToken))
                throw new ArgumentNullException("Authorization token has not been provided");

            var defaultRequestHeaders = _httpClient.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResult.AccessToken);

            return await _httpClient.GetAsync(requestUrl);
        }

    }
}
