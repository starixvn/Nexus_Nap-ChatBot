namespace Fanex.NCB.Core.Models
{
    using System.Collections.Generic;

    public class Menu
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int Policy { get; set; }

        public short DisplayOrder { get; set; }

        public IEnumerable<Menu> SubMenus { get; set; }
    }
}