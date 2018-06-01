using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// OAuth2.0客户端访问帮助类
    /// </summary>
    public class OAuthClient
    {
        private  HttpClient httpClient;
        /// <summary>
        /// 访问资源服务器的HTTP客户端
        /// </summary>
        public HttpClient ResourceServerClient { get; private set; }
        /// <summary>
        /// 操作过程中的异常信息
        /// </summary>
        public string ExceptionMessage { get; private set; }
        /// <summary>
        /// 相关的会话标识
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 采用配置文件默认初始化
        /// </summary>
        public OAuthClient()
        {
            string host_AuthorizationCenter = System.Configuration.ConfigurationManager.AppSettings["Host_AuthorizationCenter"];
            if (string.IsNullOrEmpty(host_AuthorizationCenter))
                throw new Exception("请在appSettings 配置Key为Host_AuthorizationCenter 的授权服务器基地址，示例：http://localhost:60186");

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host_AuthorizationCenter);

            string host_Webapi = System.Configuration.ConfigurationManager.AppSettings["Host_Webapi"];
            if (string.IsNullOrEmpty(host_Webapi))
                throw new Exception("请在appSettings 配置Key为Host_Webapi 的资源服务器基地址，示例：http://localhost:62477");

            ResourceServerClient = new HttpClient();
            ResourceServerClient.BaseAddress = new Uri(host_Webapi);
        }
        /// <summary>
        /// 以授权服务器地址初始化本类
        /// </summary>
        /// <param name="hostAddress">授权服务器地址</param>
        public OAuthClient(string hostAddress)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(hostAddress);
        }

        /// <summary>
        /// 获取或者设置当前的访问令牌。建议执行本类的GetToken 方法后设置该值
        /// </summary>
        public TokenResponse CurrentToken { get; set; }

       

        /// <summary>
        /// 获取密码模式的访问令牌
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="validationCode">验证码</param>
        /// <returns></returns>
        public Task<TokenResponse> GetTokenOfPasswardGrantType(string userName, string password, string validationCode)
        {
            string scope = string.Format("SessionID:{0} ValidationCode:{1}", this.SessionID, validationCode);
            return GetToken("password", null, userName, password,null, scope);
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="grantType">授权模式</param>
        /// <param name="refreshToken">刷新的令牌</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="authorizationCode">授权码</param>
        /// <param name="scope">可选业务参数</param>
        /// <returns></returns>
         public  async Task<TokenResponse> GetToken(string grantType, string refreshToken = null, string userName = null, string password = null, string authorizationCode = null,string scope=null)
        {
            var clientId = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
            var clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
            this.ExceptionMessage = "";
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grantType);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("username", userName);
                parameters.Add("password", password);
                parameters.Add("scope", scope);
           
            }
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                var redirect_uri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                parameters.Add("code", authorizationCode);
                parameters.Add("redirect_uri", redirect_uri); //和获取 authorization_code 的 redirect_uri 必须一致，不然会报错
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
            string errCode = "00";
            try
            {
                //PostAsync 在ASP.NET下面，必须加).ConfigureAwait(false)；否则容易导致死锁
                //详细内容，请参考 http://blog.csdn.net/ma_jiang/article/details/53887967
                var cancelTokenSource = new CancellationTokenSource(50000);
                var response = await httpClient.PostAsync("/api/token", new FormUrlEncodedContent(parameters), cancelTokenSource.Token).ConfigureAwait(false);
                var responseValue = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    try
                    {
                        var error = await response.Content.ReadAsAsync<HttpError>();
                        if (error.ExceptionMessage == null)
                        {
                            string errMsg = "";
                            foreach (var item in error)
                            {
                                errMsg += "\""+item.Key + "\":\"" + (item.Value == null ? "" : item.Value.ToString()) + "\",";
                            }
                            this.ExceptionMessage = "\"HttpError\":{" + errMsg.TrimEnd(',')+"}";
                        }
                        else
                        {
                            this.ExceptionMessage = error.ExceptionMessage;
                        }
                        errCode = "1000";
                    }
                    catch (AggregateException agex)
                    {
                        string errMsg = "";
                        foreach (var ex in agex.InnerExceptions)
                        {
                            errMsg += ex.Message;
                        }

                        errCode = "1001";
                        this.ExceptionMessage = errMsg;
                    }
                    catch (Exception ex)
                    {
                        this.ExceptionMessage = response.Content.ReadAsStringAsync().Result;
                        errCode = "1002";
                        WriteErrorLog(errCode, ex.Message);
                    }

                    WriteErrorLog(errCode, "StatusCode:" + response.StatusCode + "\r\n" + this.ExceptionMessage);
                    this.ExceptionMessage = "{\"ErrorCode\":" + errCode + ",\"ErrorObject\":{" + this.ExceptionMessage + "}}";
                    return null;
                }
                return await response.Content.ReadAsAsync<TokenResponse>();
            }
            catch (AggregateException agex)
            {
                string errMsg = "";
                foreach (var ex in agex.InnerExceptions)
                {
                    errMsg += ex.Message+",";
                }

                errCode = "1003";
                this.ExceptionMessage = errMsg;
                WriteErrorLog(errCode, errMsg);
                this.ExceptionMessage = "{\"ErrorCode\":" + errCode + ",\"ErrorMessage\":\"" + this.ExceptionMessage + "\"}";
                return null;
            }
            catch (Exception ex)
            {
                this.ExceptionMessage = ex.Message;
                errCode = "1004";
                WriteErrorLog(errCode, this.ExceptionMessage);
                this.ExceptionMessage = "{\"ErrorCode\":" + errCode + ",\"ErrorMessage\":\"" + this.ExceptionMessage + "\"}";
                return null;
            }
        }

        /// <summary>
        /// 使用当前令牌，尝试刷新访问令牌
        /// </summary>
        /// <returns></returns>
         public async Task<TokenResponse> RefreshToken()
         {
             if (this.CurrentToken == null)
                 throw new Exception("请先调用GetToken 方法获取令牌，然后设置");
             //如果当前令牌已经过期，再刷新
             if (!TestToken(this.CurrentToken))
                 return await GetToken("refresh_token", this.CurrentToken.RefreshToken);
             else
                 return this.CurrentToken;
         }

         /// <summary>
         /// 使用指定的令牌，直接刷新访问令牌
         /// </summary>
         /// <param name="token"></param>
         /// <returns></returns>
         public TokenResponse RefreshToken(TokenResponse token)
         {
             this.CurrentToken = token;
             return  GetToken("refresh_token", token.RefreshToken).Result;
         }

        public async Task<TokenResponse> RefreshToken(TokenResponse token,bool notest)
        {
            this.CurrentToken = token;
            if (!notest)
                return await RefreshToken();
            else
                return await GetToken("refresh_token", token.RefreshToken);
        }

        /// <summary>
        /// 返回一个携带新令牌的资源访问客户端对象。
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public  HttpClient GetResourceClient(TokenResponse token)
         {
             this.CurrentToken = token;
             SetAuthorizationRequest(this.ResourceServerClient, token);
             return this.ResourceServerClient;
         }

        /// <summary>
        /// 使用资源服务器和访问令牌信息构造一个新的资源服务访问客户端对象
        /// </summary>
         /// <param name="hostWebapi">资源服务器基地址</param>
        /// <param name="token">之前的访问令牌</param>
        /// <returns></returns>
         public async Task<HttpClient> GetResourceClient(string hostWebapi, TokenResponse token)
         {
             ResourceServerClient = new HttpClient();
             ResourceServerClient.BaseAddress = new Uri(hostWebapi);
             this.CurrentToken = token;
             var newToken = await RefreshToken();
             SetAuthorizationRequest(ResourceServerClient, newToken);
             return ResourceServerClient;
         }

         /// <summary>
         /// 测试令牌是否有效
         /// </summary>
         /// <param name="token"></param>
         /// <returns></returns>
         public bool TestToken(TokenResponse token)
         { 
           httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token.AccessToken);
            var response = httpClient.GetAsync("/api/AccessToken").Result;
            return response.StatusCode == HttpStatusCode.OK;
         }

        /// <summary>
        /// 获取授权码
        /// </summary>
        /// <returns></returns>
        public  async Task<string> GetAuthorizationCode()
        {
            var clientId = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
            var redirect_uri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
            string authorization_code_url = HttpUtility.UrlEncode(redirect_uri); //"http://localhost:8001/api/authorization_code"
            string urlFormate="/authorize?grant_type=authorization_code&response_type=code&client_id={0}&redirect_uri={1}";
            string targetUrl = string.Format(urlFormate,clientId,authorization_code_url);
            var response = await httpClient.GetAsync(targetUrl);
            var authorizationCode = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                return null;
            }
            return authorizationCode;
        }

        public void SetAuthorizationRequest(HttpClient httpClient,TokenResponse token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        public async Task OpenUrlByBrowser(string userName, string url)
        {
            HttpClient client = this.ResourceServerClient;
            string token = await client.GetStringAsync("/Logon/GetUserToken");
            Uri uri = new Uri(url);
            string targetUrl = string.Format("http://{0}/Logon/ValidateUserToken?userName={1}&token={2}&redirUrl={3}",
                uri.Authority,
                userName,
                token,
                Uri.EscapeUriString(uri.PathAndQuery + uri.Fragment));
            Process.Start(targetUrl);
        }

        public async Task<HttpStatusCode> WebLogin(string userName, string password, Action<LogonResultModel> logonResult)
        {
            HttpClient client = this.ResourceServerClient;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("用户名不能为空！");
            }

            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("username", userName);
                parameters.Add("password", password);
            }

            var response = await client.PostAsync("/Logon", new FormUrlEncodedContent(parameters));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LogonResultModel>();
                //if (result.LogonMessage == "OK")
                //{
                //    MessageBox.Show("登录成功！");
                //    //有关 cookie，可以参考：
                //    // string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
                //    // http://www.cnblogs.com/leeairw/p/3754913.html
                //    // http://www.cnblogs.com/sjns/p/5331723.html
                //}
                //else
                //{
                //    MessageBox.Show(result.LogonMessage);
                //}
                logonResult(result);
            }
            return response.StatusCode;
        }

        /// <summary>
        /// 写入异常日志文件
        /// </summary>
        /// <param name="errCode">错误代码</param>
        /// <param name="logText"></param>
        public static void WriteErrorLog(string errCode, string logText)
        {
            string filePath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "ProxyErrorLog.txt");
            try
            {
                string text = string.Format("{0} ErrorCode:{1} ErrorMsg:{2}\r\n", DateTime.Now.ToString(), errCode, logText);
                System.IO.File.AppendAllText(filePath, text);
            }
            catch
            {

            }
        }
       
    }
}
