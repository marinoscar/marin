using Luval.Web.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Web
{
    public class GalleryConfigurationOptions : LuvalConfigurationOptions
    {
        public GalleryConfigurationOptions(IWebHostEnvironment environment) : base(environment)
        {

        }
    }
}
