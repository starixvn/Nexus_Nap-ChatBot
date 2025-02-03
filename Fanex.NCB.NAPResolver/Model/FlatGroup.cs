namespace Fanex.NCB.NAPResolver.Model;

using System;
using Fanex.NAP.Client.Models;

public class FlatGroup
{
    public short GroupId { get; set; }
    public string GroupName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ModifierId { get; set; }
    public string ModifierUsername { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Group ConvertToNestedModel()
    {
        return new()
        {
            Id = GroupId,
            Name = GroupName,
            Description = Description,
            IsActive = IsActive,
            CreatedDate = CreatedDate,
            Creator = new User { Id = CreatorId, Name = CreatorUsername },
            ModifiedDate = ModifiedDate,
            Modifier = new User { Id = ModifierId, Name = ModifierUsername }
        };
    }
}