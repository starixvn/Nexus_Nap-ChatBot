using System.Collections.Generic;

namespace Fanex.NCB.Common.DTO
{
    public class Menu
    {
        public int Id {get; set;}
        public string Name { get; set; }
        public string Url { get; set; } 
        public int Policy { get; set; }
        public short DisplayOrder { get; set; }
        public List<Menu> SubMenus { get; set; }
    }
}
