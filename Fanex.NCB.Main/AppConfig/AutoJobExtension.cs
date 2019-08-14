namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.NCB.AutoJobs.SendNotification;
    using Microsoft.Extensions.DependencyInjection;

    public static class AutoJobExtension
    {
        public static void InitAutoJobs(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var autoSendNoti = serviceProvider.GetService<ScanAndSendNotificationService>();
            autoSendNoti.Initialize();
        }

        public static void InjectAutoJobService(this IServiceCollection services)
        {
            services.AddTransient<ScanAndSendNotificationService>();
        }
    }
}