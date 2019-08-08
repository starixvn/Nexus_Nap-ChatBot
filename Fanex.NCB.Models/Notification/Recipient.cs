namespace Fanex.NCB.Models.Notification
{
    public class Recipient
    {
        public int RecipientTypeId { get; set; }

        public string RecipientTypeName { get; set; }

        public int? LicenseeTypeId { get; set; }

        public string LicenseeTypeName { get; set; }

        public int RecipientId { get; set; }

        public string RecipientName { get; set; }
    }
}