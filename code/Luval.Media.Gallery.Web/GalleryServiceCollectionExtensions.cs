using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Web
{
    public static class GalleryServiceCollectionExtensions
    {
        public static void AddGallery(this IServiceCollection services, string sqlConnectionString, OAuthAuthoizationOptions authoizationOptions)
        {
            services.ConfigureOptions(typeof(GalleryConfigurationOptions));
            services.AddTransient<IMediaGalleryRepository>((sp) => {
                return new MediaGalleryRepository();
            });
            services.AddSingleton<OAuthAuthoizationOptions>(authoizationOptions);
        }
    }
}
