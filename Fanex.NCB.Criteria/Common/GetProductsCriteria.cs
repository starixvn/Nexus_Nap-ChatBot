namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

public class GetProductsCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_Product_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}