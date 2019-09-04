namespace Fanex.NCB.Shared.Models.NapUserControl
{
    using System;

    public class NapBaseEntity
    {
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}