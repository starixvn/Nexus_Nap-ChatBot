namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl
{
    using System;

    public class NapOnUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SiteId { get; set; }

        public DateTime? ReloadTime { get; set; }

        public bool IsKickedOut { get; set; }

        public string IP { get; set; }

        public string NetSessionId { get; set; }
    }
}