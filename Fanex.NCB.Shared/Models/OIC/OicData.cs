namespace Fanex.NCB.Shared.Models.OIC
{
    using System.Collections.Generic;

    public class OicData
    {
        public IEnumerable<Licensee> Licensees { get; set; }

        public IEnumerable<LicenseeProduct> LicenseeProducts { get; set; }

        public IEnumerable<LicenseeStatus> LicenseeStatuses { get; set; }

        public IEnumerable<LicenseeType> LicenseeTypes { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}