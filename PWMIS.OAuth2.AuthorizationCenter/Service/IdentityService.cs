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
                System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
                var parameters = new Dictionary<string, string>();
                //parameters.Add("ID", "");
                parameters.Add("UserName", userName);
                parameters.Add("Password", password);
                //parameters.Add("Roles", "");
                string loginUrl = System.Configuration.ConfigurationManager.AppSettings["IdentityWebAPI"];
                HttpClient httpClient = new HttpClient();
                LoginResultModel result = null;
                sp.Start();
                var response = await httpClient.PostAsync(loginUrl, new FormUrlEncodedContent(parameters));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    result = new LoginResultModel();
                    result.UserName = userName;
                    try
                    {
                        result.ErrorMessage = response.Content.ReadAsAsync<HttpError>().Result.ExceptionMessage;
                    }
                    catch 
                    {
                        result.ErrorMessage = "登录错误（错误信息无法解析），服务器状态码："+response.StatusCode;
                    }
                }
                else
                {
                    result = await response.Content.ReadAsAsync<LoginResultModel>();
                }

                sp.Stop();
                if (!string.IsNullOrEmpty(result.ErrorMessage) || sp.ElapsedMilliseconds > 100)
                    WriteLog(result, sp.ElapsedMilliseconds);

                return result;
            }
        }

        public static void WriteLog(LoginResultModel result,long logTime)
        {
            string filePath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "UserLog.txt");
            try
            {
                string text = string.Format("{0} User :{1} Web Login used time(ms):{2}, ErrorMsg:{3}\r\n", DateTime.Now.ToString(), 
                    result.UserName, logTime, result.ErrorMessage);

                System.IO.File.AppendAllText(filePath, text);
            }
            catch
            {

            }
        }
    }
}