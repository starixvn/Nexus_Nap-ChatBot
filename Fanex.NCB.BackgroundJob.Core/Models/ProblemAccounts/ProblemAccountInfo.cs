namespace Fanex.NCB.AutoJobs.Core.Models.ProblemAccounts;

using System;

public class ProblemAccountInfo
{
    public int LogId { get; set; }

    public int CustId { get; set; }

    public string Username { get; set; }

    public string Username2 { get; set; }

    public string SiteName { get; set; }

    public string CategoryId { get; set; }

    public string CustomerClass { get; set; }

    public DateTime CreatedDate { get; set; }

    public string IsTest { get; set; }
}