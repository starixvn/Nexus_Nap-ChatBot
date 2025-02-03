namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket;

public class SpecialDeleteIssueType : Message
{
    public int IssueType { get; set; }

    public string LeagueNameEN { get; set; }

    public string LeagueNameCS { get; set; }
}