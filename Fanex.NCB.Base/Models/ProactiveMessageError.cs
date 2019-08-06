namespace Fanex.NCB.Shared.Models
{
    public class ProactiveMessageError
    {
        public string ConversationId { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
