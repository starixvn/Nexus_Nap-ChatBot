namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl
{
    public class NapMenuItem : NapBaseEntity
    {
        public int MenuItemId { get; set; }

        public int FunctionId { get; set; }

        public int SiteId { get; set; }

        public string MenuItemName { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public int Policy { get; set; }

        public string Url { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public string Environment { get; set; }
    }
}