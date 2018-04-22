using Demo.OAuth2.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Demo.OAuth2.Mvc.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserModel loginModel)
        {
            //由于是登录之前，这里的ID就是会话ID
            string sessionId = loginModel.ID;
            string vcode = Session["ValidateCode"] == null ? "" : Session["ValidateCode"].ToString();
            /*
            //使用缓存的方式
            string cache_key = sessionId + "_ValidateCode";
            string vcode = HttpContext.Cache[cache_key] == null ? "" : HttpContext.Cache[cache_key].ToString();
            */
            LoginResultModel result = new LoginResultModel();
           
            if (!string.IsNullOrEmpty(loginModel.ValidationCode) && loginModel.ValidationCode == vcode)
            {
                result.UserName = loginModel.UserName;
                result.ID = loginModel.ID;

            }
            else
            {
                result.ErrorMessage = "验证码错误";
            }
            
            return Json(result);
        }

        public string CreateValidate()
        {
            string vcode = (new Random().Next(100000, 999999)).ToString();
            Session["ValidateCode"] = vcode;
            /*
            //使用缓存的方式
            string cache_key = Session.SessionID + "_ValidateCode";
            HttpContext.Cache.Insert(cache_key,vcode,
                null,DateTime.Now.AddMinutes(10),Cache.NoSlidingExpiration, CacheItemPriority.Low,null);
            */
            return vcode;
        }
	}
}