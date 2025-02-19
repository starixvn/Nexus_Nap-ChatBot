namespace Fanex.NCB.Criteria.SettingConfiguration;

using Fanex.NCB.Shared.Models;

public class CreateMappingCommand : BaseCommand
{
    public int JobId { get; set; }

    public int NotificationTypeId { get; set; }

    public int NotificationTemplateId { get; set; }

    public int MsgContentsType { get; set; }

    public string RequiredParam { get; set; }

    public bool IsTest { get; set; }

    public int CreatorId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_Mapping_Ins";
    }

    public override bool IsValid()
    {
        return JobId > 0 && NotificationTypeId > 0 && NotificationTemplateId > 0;
    }
}