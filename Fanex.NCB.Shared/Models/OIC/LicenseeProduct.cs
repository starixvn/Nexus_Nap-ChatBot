namespace Fanex.NCB.Shared.Models.OIC
{
    using System;

    public class LicenseeProduct
    {
        public int LicenseeId { get; set; }

        public int ProductId { get; set; }

        public DateTime? LaunchDate { get; set; }

        public DateTime? TerminationDate { get; set; }
    }
}