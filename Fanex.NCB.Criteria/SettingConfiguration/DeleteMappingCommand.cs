namespace Fanex.NCB.Criteria.SettingConfiguration;

using Fanex.NCB.Shared.Models;

public class DeleteMappingCommand : BaseCommand
{
    public int JobId { get; set; }

    public int NotificationTemplateId { get; set; }

    public int MsgContentsType { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_Mapping_Del";
    }

    public override bool IsValid()
    {
        return JobId > 0 && NotificationTemplateId > 0 && ModifiedBy > 0;
    }
}