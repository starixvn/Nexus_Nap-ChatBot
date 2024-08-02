namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl
{
    public class NapGroup : NapBaseEntity
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}