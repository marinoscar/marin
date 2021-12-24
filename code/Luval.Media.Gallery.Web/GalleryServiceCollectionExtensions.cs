using Luval.Common.Security;
using Luval.Data.Sql;
using Luval.Media.Gallery.Entities;
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
            var factory = new SqlServerUnitOfWorkFactory(sqlConnectionString);

            services.ConfigureOptions(typeof(GalleryConfigurationOptions));
            services.AddTransient<IMediaGalleryRepository>((sp) => {
                
                return new MediaGalleryRepository(factory.Create<MediaItem, string>());
            });

            services.AddTransient<IGraphAutenticationRepository>((sp) => {

                return new GraphAutenticationRepository(factory.Create<GraphAuthenticationToken, string>());
            });

            services.AddTransient<ISafeItemRepository>((sp) => {
                return new SafeItemRepository(factory.Create<SafeItem, string>());
            });

            services.AddSingleton<OAuthAuthoizationOptions>(authoizationOptions);
        }
    }
}
