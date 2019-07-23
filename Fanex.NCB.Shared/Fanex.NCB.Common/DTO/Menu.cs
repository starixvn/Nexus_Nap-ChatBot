using System.Collections.Generic;

namespace Fanex.NCB.Common.DTO
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
#pragma warning disable S3996
        public string Url { get; set; }
#pragma warning restore S3996
        public int Policy { get; set; }
        public short DisplayOrder { get; set; }
        public IEnumerable<Menu> SubMenus { get; set; }
    }
}