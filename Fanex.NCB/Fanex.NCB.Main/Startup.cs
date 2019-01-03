using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.NCB.Core.Models.DataDomain;
using Fanex.NCB.Core.Services;
using Fanex.NCB.SkypeBot.App_Start;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NCB.Main
{
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
            //services.Configure<CookiePolicyOptions>(options =>
            //{
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "10.18.200.103:63799"; // move to app setting later
                options.InstanceName = "Fanex.NCB.State";
            });
            services.AddSession(configure =>
            {
                configure.Cookie = new CookieBuilder
                {
                    Name = "Fanex.NCB.Session",
                    Expiration = TimeSpan.FromMinutes(60),
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Index";
                options.LogoutPath = "/Auth/SignOut";
                options.Cookie = new CookieBuilder
                {
                    Name = "Fanex.NCB.Auth",
                    Expiration = TimeSpan.FromMinutes(60),
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBridgeClientService, BridgeClientService>();
            services.AddSingleton<ISessionService, SessionService>();
            InitializeModules(services);
        }

        private static void InitializeModules(IServiceCollection services){
            SkypeBotDependencyInitializer.Initialize(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
