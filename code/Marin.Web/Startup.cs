using Luval.BlobStorage.Web;
using Luval.Blog.Web;
using Luval.Common;
using Luval.Data.Interfaces;
using Luval.Data.Sql;
using Luval.Web.Console;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Marin.Web
{
    /// <summary>
    /// Security Configuration
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-5.0
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper.RegisterProvider(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            if (!string.IsNullOrWhiteSpace(ConfigHelper.Get("MarinTest", true)))
                throw new ApplicationException("CONFIG WORKS {0}".Format(ConfigHelper.Get("Marin:Test", true)));

            services.AddControllersWithViews();

            var database = new Database(() =>
            {
                return new SqlConnection()
                {
                    ConnectionString = ConfigHelper.Get("UserProfile")
                };
            });
            var unitOfWorkFactory = new DbUnitOfWorkFactory(database, new SqlServerDialectFactory());

            services.AddSingleton<IUnitOfWorkFactory>(unitOfWorkFactory);
            services.AddSingleton<IApplicationUserRepository>(new ApplicationUserRepository(new DbUnitOfWorkFactory(database, new SqlServerDialectFactory())));

            //Sample configuration
            //https://github.com/mobiletonster/authn

            // Set Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.IsEssential = true;
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/denied";
                options.Events = new CookieAuthenticationEvents()
                {
                    OnSigningIn = options.UpdatePrincipalOnSignIn
                };
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = ConfigHelper.Get("Authentication:Microsoft:ClientId");
                options.ClientSecret = ConfigHelper.Get("Authentication:Microsoft:ClientSecret");
                options.CallbackPath = "/Home/Index";
                options.SaveTokens = true;
            });

            services.AddRazorPages();

            //Add the web console razor library
            services.AddWebConsole();
            services.AddBlobStorage(ConfigHelper.Get("BlobStorage:ConnectionString"), ConfigHelper.Get("BlobStorage:Container"));
            services.AddBlog(ConfigHelper.Get("UserProfile"));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
