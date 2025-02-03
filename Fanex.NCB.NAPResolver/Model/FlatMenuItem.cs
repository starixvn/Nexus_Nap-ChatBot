namespace Fanex.NCB.NAPResolver.Model;

using System;
using Fanex.NAP.Client.Models;

public class FlatMenuItem
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public string Description { get; set; }
    public int ParentId { get; set; }
    public string ParentName { get; set; }
    public short FunctionId { get; set; }
    public string FunctionName { get; set; }
    public byte SiteId { get; set; }
    public string SiteName { get; set; }
    public int CreatorId { get; set; }
    public string CreatorUserName { get; set; }
    public int ModifierId { get; set; }
    public string ModifierUsername { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public short DisplayOrder { get; set; }
    public string Environment { get; set; }
    public string Url { get; set; }
    public int Policy { get; set; }

    public MenuItem ConvertToNestedModel()
    {
        return new()
        {
            Id = MenuItemId,
            Name = MenuItemName,
            Description = Description,
            Parent = new MenuItem { Id = ParentId, Name = ParentName },
            Function = new Function { Id = FunctionId, Name = FunctionName },
            Site = new Site { Id = SiteId, Name = SiteName },
            CreatedDate = CreatedDate,
            Creator = new User { Id = CreatorId, Name = CreatorUserName },
            ModifiedDate = ModifiedDate,
            Modifier = new User { Id = ModifierId, Name = ModifierUsername },
            Environment = Environment,
            Url = Url,
            Policy = Policy,
            IsActive = IsActive,
            DisplayOrder = DisplayOrder
        };
    }
}