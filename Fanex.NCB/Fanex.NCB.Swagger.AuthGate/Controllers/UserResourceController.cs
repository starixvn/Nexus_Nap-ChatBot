using System.Linq;
using System.Web.Http;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Service.Interface;

namespace Fanex.NCB.Swagger.AuthGate.Controllers
{
    //[CustomAuthorize]

    public class UserResourceController : ApiController
    {
        private readonly IUserService _userService;
        public UserResourceController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: api/ExcelHandler/5
        [HttpGet]
        public dynamic Query(int id, [FromUri]int siteId, [FromUri] string env)
        {
            var menuItems = _userService.GetMenuItem(id, siteId, env, true);
            if (menuItems.Any())
            {
                return new
                {
                    Menu = menuItems.Select(x => CreateMenu(x)).ToList()
                };
            }

            return null;
        }

        private dynamic CreateMenu(MenuItem x)
        {
            if (x != null)
            {
                return new
                {
                    x.Url,
                    x.DisplayOrder,
                    x.Policy,
                    x.Name,
                    Parent = CreateMenu(x.Parent),
                    x.Function.Key
                };
            }
            return null;
        }
    }
}
