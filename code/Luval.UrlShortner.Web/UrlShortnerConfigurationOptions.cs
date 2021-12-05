using Luval.Web.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web
{
    public class UrlShortnerConfigurationOptions : LuvalConfigurationOptions
    {

        public UrlShortnerConfigurationOptions(IWebHostEnvironment environment) : base(environment)
        {

        }
    }
}
