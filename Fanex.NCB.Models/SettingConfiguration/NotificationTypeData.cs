namespace Fanex.NCB.Models.SettingConfiguration;

using System.Collections.Generic;
using Notification;

public class NotificationTypeData
{
    public string SystemName { get; set; }

    public string RecipientTypeNames { get; set; }

    public IEnumerable<Template> Templates { get; set; }
}