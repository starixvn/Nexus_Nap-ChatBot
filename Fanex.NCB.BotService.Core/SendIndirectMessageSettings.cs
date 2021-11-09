namespace Fanex.NCB.BotService.Core
{
    public class SendIndirectMessageSettings
    {
        public bool IsEnabled { get; set; }
        public string AppCredentialId { get; set; }
        public string AppCredentialPassword { get; set; }
        public string AuthenticateUrl { get; set; }
        public string TelegramUrl { get; set; }
    }
}