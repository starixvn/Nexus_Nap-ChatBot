namespace Fanex.NCB.Shared.Models
{
    public class ProactiveMessageError
    {
        public int NotificationId { get; set; }

        public string ConversationId { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}