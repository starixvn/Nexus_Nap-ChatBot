namespace Fanex.NCB.Models.Setting
{
    using Fanex.NCB.Core.Models;

    public class Recipient : BaseModel
    {
        public int RecipientTypeId { get; set; }

        public string Status { get; set; }
    }
}