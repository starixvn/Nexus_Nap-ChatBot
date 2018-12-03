using Fanex.NAP.Core.Models;
using Fanex.NCB.AuthServer.Models.MetalModel;
using System.Collections.Generic;

namespace Fanex.NCB.AuthServer.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuItem> GetMenuItems(int userId);

        IEnumerable<MenuMetaModel> GetMenuMetaModels(int userId);
    }
}