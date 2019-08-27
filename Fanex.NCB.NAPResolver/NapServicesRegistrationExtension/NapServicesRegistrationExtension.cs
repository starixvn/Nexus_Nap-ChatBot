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

            services.AddSingleton<GroupService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<SecurityService>();
            services.AddSingleton<SiteService>();

            services.AddSingleton<LocalUserService>();
            services.AddSingleton<LocalSiteService>();
            services.AddSingleton<LocalSecurityService>();
            services.AddSingleton<LocalGroupService>();

            services.AddSingleton<Func<NAPResolverType, IUserService>>(serviceProvider => type =>
            {
                if (type == NAPResolverType.Local)
                {
                    return serviceProvider.GetService<LocalUserService>();
                }

                return serviceProvider.GetService<UserService>();
            });

            services.AddSingleton<Func<NAPResolverType, IGroupService>>(serviceProvider => type =>
            {
                if (type == NAPResolverType.Local)
                {
                    return serviceProvider.GetService<LocalGroupService>();
                }

                return serviceProvider.GetService<GroupService>();
            });

            services.AddSingleton<Func<NAPResolverType, ISecurityService>>(serviceProvider => type =>
            {
                if (type == NAPResolverType.Local)
                {
                    return serviceProvider.GetService<LocalSecurityService>();
                }

                return serviceProvider.GetService<SecurityService>();
            });

            services.AddSingleton<Func<NAPResolverType, ISiteService>>(serviceProvider => type =>
            {
                if (type == NAPResolverType.Local)
                {
                    return serviceProvider.GetService<LocalSiteService>();
                }

                return serviceProvider.GetService<SiteService>();
            });
        }
    }
}