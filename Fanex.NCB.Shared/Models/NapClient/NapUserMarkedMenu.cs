namespace Fanex.NCB.Shared.Models.NapClient
{
    public class NapUserMarkedMenu : NapBaseEntity
    {
        public int UserId { get; set; }

        public int MenuItemId { get; set; }

        public bool IsMarked { get; set; }
    }
}