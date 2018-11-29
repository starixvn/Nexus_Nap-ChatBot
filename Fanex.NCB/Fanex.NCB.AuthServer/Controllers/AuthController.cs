using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fanex.NCB.AuthServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController()
        {
            _userService = new UserService();
        }

        // GET: Auth
        public ActionResult Verify(string userName, string passWord)
        {
            var user = _userService.Get(userName);
            if (user != null)
            {
                return Json(new
                {
                    UserName = user.Username,
                    FullName = string.Join(" ", user.FirstName, user.LastName),
                    Menu = new List<string>() { "menu1", "menu2" },
                    Permissions = new List<string>() { "key1", "key2" },
                    Group = "Administrator"
                });
            }

            return null;
        }

    }
}