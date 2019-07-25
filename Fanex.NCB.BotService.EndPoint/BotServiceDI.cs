namespace Fanex.NCB.BotService.EndPoint
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Base.Repositories;
    using Fanex.NCB.Base.Services;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IConversationService, ConversationService>();
            services.AddTransient<IBot, LicBot>();
        }
    }
}