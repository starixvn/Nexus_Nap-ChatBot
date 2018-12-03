using Fanex.NAP.Core.Models;
using System.Collections.Generic;

namespace Fanex.NCB.AuthServer.Models.MetalModel
{
    public class MenuMetaModel
    {
        public MenuMetaModel()
        {
            this.SubMenus = new List<MenuMetaModel>();
        }

        public MenuMetaModel(MenuItem menuItem)
            : this()
        {
            this.Id = menuItem.Id;
            this.Name = menuItem.Name;
            this.Link = menuItem.Url;
            this.Policy = menuItem.Policy;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Class { get; set; }

        public string Link { get; set; }

        public int Policy { get; set; }

        public IList<MenuMetaModel> SubMenus { get; private set; }
    }
}