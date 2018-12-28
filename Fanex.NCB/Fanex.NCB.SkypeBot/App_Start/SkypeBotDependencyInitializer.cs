using Fanex.NCB.SkypeBot.Services.NotificationType;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NCB.SkypeBot.App_Start
{
    public static class SkypeBotDependencyInitializer
    {
        public static void Initialize(IServiceCollection services){
            services.AddSingleton<INotificationTypeService, NotificationTypeService>();
        }
    }
}