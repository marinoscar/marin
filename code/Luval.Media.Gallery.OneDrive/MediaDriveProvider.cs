using Azure.Identity;
using Luval.Data.Extensions;
using Luval.Media.Gallery.Entities;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class MediaDriveProvider
    {

        AuthenticationOptions _authenticationOptions;

        public MediaDriveProvider(AuthenticationOptions authenticationOptions)
        {
            _authenticationOptions = authenticationOptions;
        }


        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveAsync(MediaDrive drive, CancellationToken cancellationToken)
        {
            //fix to the issue:
            //https://github.com/OneDrive/onedrive-api-docs/issues/1266#issuecomment-766941950

            var auth = await GetTokenAsync(_authenticationOptions);
            var graphClient = new GraphClient(auth);
            //var res = await graphClient.RunGraphRequestAsync("https://graph.microsoft.com/v1.0//users/06bb48d2-64a3-4818-9087-526eacae205a/drive", cancellationToken);
            var res = await graphClient.RunGraphRequestAsync("https://graph.microsoft.com/v1.0//drives/7182b0080429dbe3/items/root/children", cancellationToken);
            var content = await res.Content.ReadAsStringAsync(cancellationToken);
            Debug.WriteLine(res.StatusCode);
            return new List<MediaItem>();
        }


        private GraphServiceClient CreateClient(AuthenticationOptions authenticationOptions)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                authenticationOptions.TenantId,
                authenticationOptions.ClientId,
                authenticationOptions.ClientSecret,
                options);
            return new GraphServiceClient(clientSecretCredential, scopes);
        }

        public Task<AuthenticationResult> GetTokenAsync(AuthenticationOptions authenticationOptions)
        {
            return GetTokenAsync(authenticationOptions, CancellationToken.None);
        }

        public Task<AuthenticationResult> GetTokenAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken)
        {
            return GetTokenAsync(authenticationOptions, authenticationOptions.GetDefaultScopes(), cancellationToken);
        }

        public async Task<AuthenticationResult> GetTokenAsync(AuthenticationOptions authenticationOptions, string[] scopes, CancellationToken cancellationToken)
        {
            var validatedScopes = authenticationOptions.GetDefaultScopes();
            var app = ConfidentialClientApplicationBuilder.Create(authenticationOptions.ClientId)
                    .WithClientSecret(authenticationOptions.ClientSecret)
                    .WithAuthority(new Uri(authenticationOptions.GetAuthority()))
                    .Build();

            app.AddInMemoryTokenCache();

            if (scopes != null && scopes.Any()) validatedScopes = scopes;

            return await AcquireTokenAsync(app, validatedScopes, cancellationToken);
        }

        private async Task<AuthenticationResult> AcquireTokenAsync(IConfidentialClientApplication app, string[] scopes, CancellationToken cancellationToken)
        {
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync(cancellationToken);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                throw new ArgumentException("Scope provided is not supported", ex);
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
            }
            return result;
        }


    }
}
