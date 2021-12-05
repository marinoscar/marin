using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Common
{
    public static class LuvalServiceCollectionExtensions
    {
        public static void AddLuvalWebCommon(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(LuvalConfigurationOptions));
        }
    }
}
