namespace Fanex.NCB.BotService.Core
{
    public class AppSettings
    {
        public string AppCredentialId { get; set; }

        public string AppCredentialPassword { get; set; }

        public string Secret { get; set; }

        public string AllowedHosts { get; set; }

        public string BotFilePath { get; set; }

        public string BotFileSecret { get; set; }

        public string TokenUrl { get; set; }

        public string SkypeServiceUrl { get; set; }
    }
}