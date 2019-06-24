using Fanex.NAP.Core.Models;
using Fanex.NCB.Common.DTO;
using System.Collections.Generic;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public interface IMenuService
    {
        List<Menu> GetMenus(IEnumerable<MenuItem> menuItems);
    }
}