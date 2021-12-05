using Luval.Data.Sql;
using Luval.Web.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web
{
    public static class UrlShortnerServiceCollectionExtensions
    {
        public static void AddShortner(this IServiceCollection services, string sqlConnectionString)
        {
            services.ConfigureOptions(typeof(UrlShortnerConfigurationOptions));
            services.AddTransient<IUrlShortnerRepository>((sp) => {
                return new UrlShortnerRepository(new SqlServerUnitOfWorkFactory(sqlConnectionString));
            });
        }
    }
}
