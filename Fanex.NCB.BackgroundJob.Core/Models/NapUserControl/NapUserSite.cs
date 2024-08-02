namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl
{
    using System;

    public class NapUserSite : NapBaseEntity
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public int Level { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public bool IsNeverExpired { get; set; }
    }
}