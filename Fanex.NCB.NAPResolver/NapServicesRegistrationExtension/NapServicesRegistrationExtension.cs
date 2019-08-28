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
        public static void AddNapServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INapSettings>(new NapSettings(configuration));
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();

            var resolverType = Enum.Parse<NAPResolverType>(configuration["NapOption:NAPResolverType"]);

            if (resolverType == NAPResolverType.Remote)
            {
                services.AddSingleton<IGroupService, GroupService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<ISiteService, SiteService>();
            }
            else
            {
                services.AddSingleton<IGroupService, LocalGroupService>();
                services.AddSingleton<IUserService, LocalUserService>();
                services.AddSingleton<ISecurityService, LocalSecurityService>();
                services.AddSingleton<ISiteService, LocalSiteService>();
            }
        }
    }
}