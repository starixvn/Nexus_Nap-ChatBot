namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket
{
    public class PullMessageResult
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful { get => Code == 0; }
    }
}