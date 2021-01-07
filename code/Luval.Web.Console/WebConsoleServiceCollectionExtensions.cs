using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Console
{
    public static class WebConsoleServiceCollectionExtensions
    {
        public static void AddWebConsole(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(WebConsoleConfigureOptions));
        }
    }
}
