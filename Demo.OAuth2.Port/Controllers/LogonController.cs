using Demo.OAuth2.Port.Models;
using PWMIS.OAuth2.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Demo.OAuth2.Port.Controllers
{
    public class LogonController : Controller
    {
        //
        // GET: /Logon/
        public ActionResult Index()
        {
            ViewBag.Host_AuthorizationCenter = System.Configuration.ConfigurationManager.AppSettings["Host_AuthorizationCenter"];
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(LogonModel model)
        {
            LogonResultModel result = new LogonResultModel();
            //result.UserName="";

            //首先，调用授权服务器，以密码模式获取访问令牌
            //授权服务器会携带用户名和密码到认证服务器去验证用户身份
            //验证服务器验证通过，授权服务器生成访问令牌给当前站点程序
            //当前站点标记此用户登录成功，并将访问令牌存储在当前站点的用户会话中
            //当前用户下次访问别的站点的WebAPI的时候，携带此访问令牌。
           
            TokenManager tm = new TokenManager(model.UserName);
            var tokenResponse = await tm.CreateToken(model.Password);
            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                result.UserId = 123;
                result.UserName = model.UserName;
                result.LogonMessage = "OK";
                /* OWin的方式
                ClaimsIdentity identity = new ClaimsIdentity("Basic");
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                HttpContext.User = principal;
                */
                FormsAuthentication.SetAuthCookie(model.UserName, false);
            }
            else
            {
                result.LogonMessage = tm.TokenExctionMessage;
            }
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(LogonModel model)
        {
            return View();
        }

    }
}