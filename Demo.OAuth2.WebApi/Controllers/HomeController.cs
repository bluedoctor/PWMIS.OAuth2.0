using Demo.OAuth2.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Demo.OAuth2.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            ClaimsIdentity identity = new ClaimsIdentity("Basic");
            identity.AddClaim(new Claim(ClaimTypes.Name, "zhang san"));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
          

            HttpContext.User = principal;

            return View();
        }

        public ActionResult GetLogonUser(int userId)
        {
            LogonResultModel model = new LogonResultModel();
            model.UserId = userId;
            model.UserName = "张三";
            model.LogonMessage = "OK";
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
