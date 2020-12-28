using Luval.Data;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authentication;
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
using System.Diagnostics;
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

            //services.AddExternalMarinSignIn<ExternalUser, ExternalRole>(database);


            //possible fix
            //https://github.com/dotnet/aspnetcore/issues/18013

            // Set Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = MicrosoftAccountDefaults.AuthenticationScheme;
                //options.DefaultScheme = MicrosoftAccountDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.IsEssential = true;
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                //options.Events.OnCreatingTicket = (ctx) =>
                //{
                //    Debug.WriteLine(ctx);
                //    return options.Events.CreatingTicket(ctx);
                //};
                //options.Events.OnRedirectToAuthorizationEndpoint = (opts) =>
                //{
                //    Debug.WriteLine(opts);
                //    var task = options.Events.RedirectToAuthorizationEndpoint(opts);
                //    task.Wait();
                //    return task;
                //};
                options.Events.OnTicketReceived = (ctx) =>
                {
                    Debug.WriteLine(ctx);
                    ctx.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }));
                    var newRole = ctx.Principal.IsInRole("Admin");
                    return Task.CompletedTask;
                };
            });

            services.AddRazorPages();
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
