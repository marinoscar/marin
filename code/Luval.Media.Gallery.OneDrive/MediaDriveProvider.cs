using Azure.Identity;
using Luval.Data.Extensions;
using Luval.Media.Gallery.Entities;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class MediaDriveProvider
    {
        public MediaDriveProvider(AuthenticationOptions authenticationOptions)
        {
            Client = CreateClient(authenticationOptions);
        }

        public GraphServiceClient Client { get; private set; }

        public async Task<IEnumerable<MediaItem>> GetItemsFromDrive(MediaDrive drive, CancellationToken cancellationToken)
        {
            var items = await Client.Drives[drive.DriveId].Items[drive.DrivePath].Children.Request()
                .GetAsync(cancellationToken);
            return items.Select(i => i.ToMediaItem());
        }

        public async Task<IEnumerable<MediaItem>> GetUpdatedItemsFromDriveAsync(MediaDrive drive, DateTime timeOfLastUpdate, CancellationToken cancellationToken)
        {
            //sample filter video
            //$filter=contains(remoteItem/file/mimeType, 'video')
            //sample filter date time
            //$filter=lastModifiedDateTime gt {0}

            var datevalue = timeOfLastUpdate.ToString("O");
            var items = await Client.Drives[drive.DriveId].Items[drive.DrivePath].Children.Request()
                .Filter("lastModifiedDateTime gt {0}".FormatInvariant(datevalue))
                .GetAsync(cancellationToken);
            return items.Select(i => i.ToMediaItem());
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


    }
}
