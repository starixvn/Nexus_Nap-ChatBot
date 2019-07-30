namespace Fanex.NCB.BotService.EndPoint
{
    using Fanex.Data.Repository;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Fanex.NCB.Core.Repositories;
    using Fanex.NCB.Core.Services;
    using Fanex.NCB.Core.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IConversationService, ConversationService>();
            services.AddTransient<IBot, LicBot>();
        }
    }
}