namespace Fanex.NCB.NAPResolver.Model;

using System;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Models.ValueObject;

public class FlatSiteLevel
{
    public byte SiteId { get; set; }
    public string SiteName { get; set; }
    public string Url { get; set; }
    public string TestUrl { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastLoginDate { get; set; }
    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ModifierId { get; set; }
    public string ModifierUsername { get; set; }
    public DateTime ModifiedDate { get; set; }

    public SiteLevel ConvertToNestedModel()
    {
        return new()
        {
            Site = new Site { Id = SiteId, Name = SiteName },
            Level = Level,
            IsActive = IsActive,
            LastLoginDate = LastLoginDate,
            CreatedDate = CreatedDate,
            Creator = new User { Id = CreatorId, Name = CreatorUsername },
            ModifiedDate = ModifiedDate,
            Modifier = new User { Id = ModifierId, Name = ModifierUsername }
        };
    }
}