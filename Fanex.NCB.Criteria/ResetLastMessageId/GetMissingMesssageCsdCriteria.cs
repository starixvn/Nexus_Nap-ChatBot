namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetMissingMesssageCsdCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NAP_Chatbot_AbnormalBets_MissingMesssageInfo_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}