using Luval.Data;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;

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
            services.AddLogging();
            services.AddControllersWithViews();
            var database = new Database(() =>
            {
                return new SqlConnection()
                {
                    ConnectionString = Configuration.GetConnectionString("UserProfile")
                };
            });

            services.AddScoped<EntityAdapter<ExternalUser>>((s) =>
            {
                return new EntityAdapter<ExternalUser>(database, new SqlServerDialectFactory());
            });

            services.AddScoped<EntityAdapter<ExternalRole>>((s) =>
            {
                return new EntityAdapter<ExternalRole>(database, new SqlServerDialectFactory());
            });

            services.AddIdentity<ExternalUser, ExternalRole>()
                .AddDefaultTokenProviders()
                .AddUserStore<ExternalUserStore<ExternalUser>>()
                .AddUserManager<UserManager<ExternalUser>>()
                .AddRoleStore<ExternalRoleStore<ExternalRole>>()
                .AddRoleManager<RoleManager<ExternalRole>>();



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
            });
        }

        private void StartAuthenticationDatabaseItems(IServiceCollection services)
        {

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
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
