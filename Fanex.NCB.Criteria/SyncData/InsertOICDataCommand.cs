namespace Fanex.NCB.Criteria.SyncData
{
    using Fanex.NCB.Shared.Models;

    public class InsertOicDataCommand : BaseCommand
    {
        public string Licensees { get; set; }

        public string LicenseeProducts { get; set; }

        public string LicenseeStatuses { get; set; }

        public string LicenseeTypes { get; set; }

        public string Products { get; set; }

        public string Platforms { get; set; }

        public string IntegrationMethod { get; set; }

        public string LicenseeIntegrationMethod { get; set; }

        public override string GetSettingKey() => "NCB_OIC_SyncData";

        public override bool IsValid() => true;
    }
}