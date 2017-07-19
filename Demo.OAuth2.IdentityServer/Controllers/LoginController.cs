using Demo.OAuth2.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Demo.OAuth2.IdentityServer.Controllers
{
    public class LoginController : ApiController
    {

        // POST api/<controller>
        public LoginResultModel Post([FromBody]UserModel loginModel)
        {
            //这里假设请求的登录信息均合法有效，直接返回结果
            LoginResultModel result = new LoginResultModel();
            result.ID = Guid.NewGuid().ToString();
            result.UserName = loginModel.UserName;
            result.Roles = "admin";
            return result;
        }

       
    }
}