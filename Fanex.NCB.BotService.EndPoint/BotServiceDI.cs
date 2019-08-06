namespace Fanex.NCB.BotService.EndPoint
{
    using Fanex.Data.Repository;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IConversationService, ConversationService>();
            services.AddTransient<ICredentialProvider, MultiCredentialProvider>();
            services.AddTransient<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
            services.AddTransient<IBot, LicBot>();
        }
    }
}