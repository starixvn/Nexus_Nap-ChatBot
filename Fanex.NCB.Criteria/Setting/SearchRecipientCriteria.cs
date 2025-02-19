namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class SearchRecipientCriteria : CriteriaBase
{
    public string Keyword { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_Setting_SearchRecipient_Sel";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Keyword);
    }
}