namespace Fanex.NCB.NAPResolver.Model;

using System;
using Fanex.NAP.Client.Models;

public class FlatUser
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public short GroupId { get; set; }
    public string GroupName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime LastLoginDate { get; set; }
    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ModifierId { get; set; }
    public string ModifierUsername { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public User ConvertFlatUserToNestedModel()
    {
        return new()
        {
            Id = UserId,
            Name = Name,
            Username = Username,
            Password = Password,
            Group = new Group { Id = GroupId, Name = GroupName },
            LastLoginDate = LastLoginDate,
            Creator = new User { Id = CreatorId, Name = CreatorUsername },
            CreatedDate = CreatedDate,
            Modifier = new User { Id = ModifierId, Username = ModifierUsername },
            ModifiedDate = ModifiedDate,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email
        };
    }
}