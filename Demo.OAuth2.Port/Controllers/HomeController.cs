using PWMIS.OAuth2.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Demo.OAuth2.Port.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ClaimsIdentity identity = new ClaimsIdentity("Windows");
            //identity.AddClaim(new Claim(ClaimTypes.Name, "zhang san"));

            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            //FormsAuthentication.SetAuthCookie("zhagn san", true);
            //HttpContext.User = principal;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(string who="")
        {
            ViewBag.Message = "Welcom "+who+",Your contact page.";

            return View();
        }

        [Authorize]
        public async Task<ActionResult> Biz()
        {
            ViewBag.Message = "当前登录用户：" + HttpContext.User.Identity.Name;

            //OAuthClient oc = new OAuthClient(System.Configuration.ConfigurationManager.AppSettings["Host_AuthorizationCenter"]);
            //使用默认构造函数简化配置
            OAuthClient oc = new OAuthClient();
            TokenResponse userToken = null;
            using (TokenManager tm = new TokenManager(HttpContext.User.Identity.Name))
            {
                userToken = tm.TakeToken();
                if (userToken == null)
                    return Redirect("/Logon");
           
            

                //oc.CurrentToken = token;
                //var newToken = await oc.RefreshToken();


                //string host_Webapi = System.Configuration.ConfigurationManager.AppSettings["Host_Webapi"];
                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(host_Webapi);

                //oc.SetAuthorizationRequest(client, newToken);
                //var responseTwo = await client.GetAsync("/api/values");

                //使用下面一行代码，简化上面注释的代码
                var resourceClient = oc.GetResourceClient(userToken);
                //TokenRepository.SetUserToken( oc.CurrentToken);
                var responseTwo = await resourceClient.GetAsync("/api/values");
                if (responseTwo.StatusCode != HttpStatusCode.OK)
                {
                    ViewBag.Message = "访问WebAPI 失败。";
                }
                ViewBag.Message = await responseTwo.Content.ReadAsStringAsync();
            }
            return View();
        }
    }
} 