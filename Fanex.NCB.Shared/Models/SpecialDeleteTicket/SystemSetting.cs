namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    public class SystemSetting
    {
        public int JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int PreviousMaxMessageId { get; set; }

        public int VoidingTicketTemplateId { get; set; }

        public int RejectTicketTemplateId { get; set; }

        public int RecoverTicketTemplateId { get; set; }

        public int ResettleTicketTemplateId { get; set; }
    }
}