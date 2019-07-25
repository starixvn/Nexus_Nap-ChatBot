namespace Fanex.NCB.BotService.EndPoint
{
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddTransient<IBot, LicBot>();
        }
    }
}