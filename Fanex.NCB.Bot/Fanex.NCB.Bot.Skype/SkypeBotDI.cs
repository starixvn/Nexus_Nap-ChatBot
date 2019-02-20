using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.MySQLStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NCB.Bot.Skype
{
    public class SkypeBotDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddTransient<IBotConversationRepository, BotConversationRepository>();
        }
    }
}
