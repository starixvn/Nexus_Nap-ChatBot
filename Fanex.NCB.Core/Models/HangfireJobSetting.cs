namespace Fanex.NCB.Core.Models
{
    public class HangfireJobSetting
    {
        public string JobName { get; set; }

        public bool IsEnabled { get; set; }

        public string CronExpression { get; set; }
    }
}