namespace Fanex.NCB.NAPResolver
{
    using Fanex.NAP.Client.AspNetCore.Configuration;
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Client.Transport;
    using Fanex.NAP.Client.Web;
    using Fanex.NCB.NAPResolver.LocalServices;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class NapServicesRegistrationExtension
    {
        public static NAPResolverType NAPResolverType { get; internal set; }

        public static void AddNapServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INapSettings>(new NapSettings(configuration));
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();

            NAPResolverType = Enum.Parse<NAPResolverType>(configuration["NapOption:NAPResolverType"]);
            if (NAPResolverType == NAPResolverType.Remote)
            {
                services.AddSingleton<IGroupService, GroupService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<ISiteService, SiteService>();
            }
            else
            {
                services.AddScoped<IGroupService, LocalGroupService>();
                services.AddScoped<IUserService, LocalUserService>();
                services.AddScoped<ISecurityService, LocalSecurityService>();
                services.AddScoped<ISiteService, LocalSiteService>();
            }
        }
    }
}