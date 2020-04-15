namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.NCB.Shared.Models;

    public class UpdateSettingTypeCommand : BaseCommand
    {
        public int Type { get; set; }

        public string Name { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_SettingType_Udp";

        public override bool IsValid() => Type > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}