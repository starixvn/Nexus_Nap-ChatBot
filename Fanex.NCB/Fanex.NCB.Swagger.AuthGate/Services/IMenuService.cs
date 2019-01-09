using Fanex.NAP.Core.Models;
using Fanex.NCB.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public interface IMenuService
    {
        List<Menu> GetMenus(IEnumerable<MenuItem> menuItems);
    }
}
