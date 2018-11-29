using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fanex.NCB.AuthServer.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Verify(string userName, string passWord)
        {
            if (userName == "alex")
            {
                return Json(new
                {
                    UserName = userName,
                    FullName = "Alex Nguyen"
                });
            }
            else
            {
                return null;
            }
        }
    }
}