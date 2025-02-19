namespace Fanex.NCB.AutoJobs.Core.Models.SiteMessage;

using System;

public class SiteMessageModel : SiteMessageMatchInfoModel
{
    public long MessageID { get; set; }

    public DateTime? PostTime { get; set; }

    public int MsgContentsType { get; set; }

    public string Content_en { get; set; }

    public string Content_cs { get; set; }

    public bool IsNew { get; set; }

    public bool IsTest { get; set; }

    public bool IsTestMsg { get; set; }
}