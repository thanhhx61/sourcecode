using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult adminLogin(string Account, string Password)
        {
            if (Account == "admin" && Password == "1234")
            {
                Session["admin_login"] = "Administrator";
                return Redirect("/Product/Index");
            }
            else
            {
                TempData["error_login"] = "Tài khoản hoặc mật khẩu không chính xác";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session["admin_login"] = null;
            return RedirectToAction("Login");
        }
    }
}