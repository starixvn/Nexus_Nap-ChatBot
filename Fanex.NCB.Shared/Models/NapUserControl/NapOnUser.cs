namespace Fanex.NCB.Shared.Models.NapUserControl
{
    public class NapOnUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SiteId { get; set; }

        public int ReloadTime { get; set; }

        public bool IsKickedOut { get; set; }

        public string IP { get; set; }

        public string NetSessionId { get; set; }
    }
}