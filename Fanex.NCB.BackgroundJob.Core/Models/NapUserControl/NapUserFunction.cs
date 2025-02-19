namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl;

public class NapUserFunction : NapBaseEntity
{
    public int UserId { get; set; }

    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public int Policy { get; set; }
}