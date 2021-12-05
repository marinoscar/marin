using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Common
{
    public class LuvalConfigurationOptions : IPostConfigureOptions<StaticFileOptions>
    {

        public LuvalConfigurationOptions(IWebHostEnvironment environment)
        {
            HostEnvironment = environment;
        }

        public IWebHostEnvironment HostEnvironment { get; private set; }

        public virtual void PostConfigure(string name, StaticFileOptions options)
        {

            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

            if (options.FileProvider == null && HostEnvironment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? HostEnvironment.WebRootFileProvider;


            // Add our provider
            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "resources");
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }
    }
}
