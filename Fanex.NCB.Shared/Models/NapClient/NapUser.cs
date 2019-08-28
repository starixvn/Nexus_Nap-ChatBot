namespace Fanex.NCB.Shared.Models.NapClient
{
    using System;

    public class NapUser : NapBaseEntity
    {
        public int UserId { get; set; }

        public int GroupId { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}