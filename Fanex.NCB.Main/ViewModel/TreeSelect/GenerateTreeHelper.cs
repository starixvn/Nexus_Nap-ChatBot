namespace Fanex.NCB.Main.ViewModel.TreeSelect
{
    using Models.Common;
    using System.Collections.Generic;
    using System.Linq;

    public static class GenerateTreeHelper
    {
        public static IEnumerable<TreeNodeItem> FromRecipient(IEnumerable<Recipient> data)

        {
            return data.GroupBy(x => new { x.RecipientTypeId, x.RecipientTypeName }, x => x, (key, list) =>
            {
                var children = list.Where(x => !x.LicenseeTypeId.HasValue).Select(x => new TreeNodeItem
                {
                    Title = x.RecipientName,
                    Value = $"{x.RecipientTypeId}-{x.RecipientId}",
                    RealValue = x.RecipientId
                }).ToList();

                children.AddRange(list.Where(x => x.LicenseeTypeId.HasValue).GroupBy(
                    x => new { x.LicenseeTypeId, x.LicenseeTypeName },
                    x => x,
                    (licensee, recipients) =>
                    {
                        return new TreeNodeItem
                        {
                            Title = licensee.LicenseeTypeName,
                            Value = $"{key.RecipientTypeId}-{licensee.LicenseeTypeId}",
                            RealValue = licensee.LicenseeTypeId.Value,
                            Children = recipients.Select(x => new TreeNodeItem
                            {
                                Title = x.RecipientName,
                                Value = $"{key.RecipientTypeId}-{licensee.LicenseeTypeId}-{x.RecipientId}",
                                RealValue = x.RecipientId
                            })
                        };
                    }));

                return new TreeNodeItem
                {
                    Title = key.RecipientTypeName,
                    Value = $"{key.RecipientTypeId}",
                    RealValue = key.RecipientTypeId,
                    Children = children
                };
            });
        }
    }
}