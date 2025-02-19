namespace Fanex.NCB.Shared.Models.NapUserControl;

public class NapFunction : NapBaseEntity
{
    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public string FunctionName { get; set; }

    public string FunctionKey { get; set; }

    public string Description { get; set; }

    public int? ParentId { get; set; }

    public string ParentName { get; set; }

    public int CRUD { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }
}