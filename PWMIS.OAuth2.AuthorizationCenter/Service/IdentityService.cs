using PWMIS.OAuth2.AuthorizationCenter.Models;
using PWMIS.OAuth2.AuthorizationCenter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PWMIS.OAuth2.AuthorizationCenter.Service
{
    public class IdentityService
    {
        public async Task<LoginResultModel> UserLogin(string userName, string password)
        { 
            //通过配置，决定是使用本地数据库验证登录，还是使用登录接口服务登录
            string identityLoginMode = System.Configuration.ConfigurationManager.AppSettings["IdentityLoginMode"];
            if (!string.IsNullOrEmpty(identityLoginMode) && identityLoginMode.ToLower() == "database")
            {
                var identityRepository = IdentityRepositoryFactory.CreateInstance();
                bool flag= await identityRepository.ValidatedUserPassword(userName, password);
                LoginResultModel result = new LoginResultModel();
                if (flag)
                {
                    result.ID = "123";
                    result.UserName = userName;
                    result.Roles = "";//暂时略
                }
                return result;
            }
            else
            {
                var parameters = new Dictionary<string, string>();
                //parameters.Add("ID", "");
                parameters.Add("UserName", userName);
                parameters.Add("Password", password);
                //parameters.Add("Roles", "");
                string loginUrl = System.Configuration.ConfigurationManager.AppSettings["IdentityWebAPI"];
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(loginUrl, new FormUrlEncodedContent(parameters));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LoginResultModel result = new LoginResultModel();
                    result.UserName = userName;
                    result.ErrorMessage = response.Content.ReadAsAsync<HttpError>().Result.ExceptionMessage;
                    return result;
                }
                else
                {
                    var result = await response.Content.ReadAsAsync<LoginResultModel>();
                    return result;
                }
            }
        }
    }
}