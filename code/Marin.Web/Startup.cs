using Luval.BlobStorage.Web;
using Luval.Blog.Web;
using Luval.Data;
using Luval.Data.Interfaces;
using Luval.Data.Sql;
using Luval.Web.Console;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Policy;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            var database = new Database(() =>
            {
                return new SqlConnection()
                {
                    ConnectionString = Configuration.GetConnectionString("UserProfile")
                };
            });
            var unitOfWorkFactory = new DbUnitOfWorkFactory(database, new SqlServerDialectFactory());


            services.AddSingleton<IUnitOfWorkFactory>(unitOfWorkFactory);
            services.AddSingleton<IApplicationUserRepository>(new ApplicationUserRepository(new DbUnitOfWorkFactory(database, new SqlServerDialectFactory())));


            //possible fix
            //https://github.com/dotnet/aspnetcore/issues/18013

            // Set Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = MicrosoftAccountDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.IsEssential = true;
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];

                options.Events.OnTicketReceived = async (ctx) =>
                {
                    var userRepo = new ApplicationUserRepository(new DbUnitOfWorkFactory(database, new SqlServerDialectFactory()));
                    try
                    {
                        await userRepo.ValidateAndUpdateUserAccess(ctx.Principal);
                    }
                    catch (Exception ex)
                    {
                        ctx.Fail(ex);
                        return;
                    }
                    ctx.Success();
                };
            });

            services.AddRazorPages();

            //Add the web console razor library
            services.AddWebConsole();
            services.AddBlobStorage(Configuration["BlobStorage:ConnectionString"], Configuration["BlobStorage:Container"]);
            services.AddBlog(Configuration.GetConnectionString("UserProfile"));
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
