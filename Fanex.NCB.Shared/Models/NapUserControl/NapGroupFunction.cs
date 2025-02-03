namespace Fanex.NCB.Shared.Models.NapUserControl;

public class NapGroupFunction : NapBaseEntity
{
    public int GroupId { get; set; }

    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public int Policy { get; set; }
}