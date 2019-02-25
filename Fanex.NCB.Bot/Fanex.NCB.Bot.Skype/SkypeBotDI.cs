using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.MySQLStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NCB.Bot.Skype
{
    public class SkypeBotDI
    {
        public static void Init(IServiceCollection services, string chatBotConnection)
        {
            services.AddDbContextPool<BotServiceDBContext>(x => x.UseMySQL(chatBotConnection));
            services.AddTransient<IBotConversationRepository, BotConversationRepository>();
            services.AddTransient<IBotConversationMemberRepository, BotConversationMemberRepository>();

        }
    }
}
