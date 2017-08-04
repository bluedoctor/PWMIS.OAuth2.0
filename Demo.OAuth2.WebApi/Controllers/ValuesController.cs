using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Demo.OAuth2.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            //string token = this.ControllerContext.Request.Headers.Authorization.Parameter;

            return new string[] { "Resource Server:",this.User.Identity.Name, 
                this.User.Identity.IsAuthenticated.ToString(), 
                this.User.Identity.AuthenticationType,
                DateTime.Now.ToLongTimeString()};
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "Resource Server: value="+id;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        //static bool TryGetAccessToken(this HttpRequestMessage request, out string accessToken)
        //{
        //    //从Cookie中获取Access Token
        //    //accessToken = null;
        //    //CookieHeaderValue cookieValue = request.Headers.Authorization..GetCookies(AuthenticateAttribute.CookieName).FirstOrDefault();
        //    //if (null != cookieValue)
        //    //{
        //    //    accessToken = cookieValue.Cookies.FirstOrDefault().Value;
        //    //    return true;
        //    //}

        //    //从查询字符串中获取Access Token
        //    accessToken = HttpUtility.ParseQueryString(request.RequestUri.Query)["access_token"];
        //    return !string.IsNullOrEmpty(accessToken);
        //}
    }
}
