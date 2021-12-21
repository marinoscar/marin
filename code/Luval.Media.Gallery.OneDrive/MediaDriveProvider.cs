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
        IAuthenticationProvider _authenticationProvider;

        public MediaDriveProvider(AuthenticationOptions authenticationOptions)
        {
            _authenticationOptions = authenticationOptions;
        }

        public MediaDriveProvider(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
        }


        public async Task<IEnumerable<MediaItem>> GetItemsFromDriveAsync(MediaDrive drive, CancellationToken cancellationToken)
        {
            //fix to the issue:
            //https://github.com/OneDrive/onedrive-api-docs/issues/1266#issuecomment-766941950

            var graphClient = new GraphServiceClient(_authenticationProvider);
            var drives = await graphClient.Me.Drive.Root.Children.Request().GetAsync(cancellationToken);

            return new List<MediaItem>();
        }
    }
}
