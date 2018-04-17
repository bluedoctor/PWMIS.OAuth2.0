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
        private static Dictionary<string, DateTime> dictValidateCode = new Dictionary<string, DateTime>();
        // POST api/<controller>
        public LoginResultModel Post([FromBody]UserModel loginModel)
        {
            LoginResultModel result = new LoginResultModel();
            //检查验证码
            if (dictValidateCode.ContainsKey(loginModel.ValidationCode))
            {
                result.ID = loginModel.ID;
                result.UserName = loginModel.UserName;
                result.Roles = "admin";

                dictValidateCode.Remove(loginModel.ValidationCode);
            }
            else
            {
                result.ErrorMessage = "验证码错误";
            }
            //这里假设请求的登录信息均合法有效，直接返回结果
            return result;
        }

        //Get api/Login/CreateValidate

        /// <summary>
        /// 生成6位数字验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string CreateValidate()
        {
            string vcode = "";
            do
            {
                vcode = (new Random().Next(100000, 999999)).ToString();
            }
            while (dictValidateCode.ContainsKey(vcode));

            dictValidateCode.Add(vcode, DateTime.Now);
            return vcode;
        }


    }
}