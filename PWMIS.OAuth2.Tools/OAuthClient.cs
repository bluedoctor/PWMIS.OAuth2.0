using System;
using System.Collections.Generic;
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
                throw new Exception("请在appSettings 配置Key为Host_Webapi 的授权服务器基地址，示例：http://localhost:62477");

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
        /// <returns></returns>
        public Task<TokenResponse> GetTokenOfPasswardGrantType(string userName, string password)
        {
            return GetToken("password", null, userName, password);
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="grantType">授权模式</param>
        /// <param name="refreshToken">刷新的令牌</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="authorizationCode">授权码</param>
        /// <returns></returns>
         public  async Task<TokenResponse> GetToken(string grantType, string refreshToken = null, string userName = null, string password = null, string authorizationCode = null)
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
            var cancelTokenSource = new CancellationTokenSource(10000); 
            var response = await httpClient.PostAsync("/token", new FormUrlEncodedContent(parameters), cancelTokenSource.Token);
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = await response.Content.ReadAsAsync<HttpError>();
                this.ExceptionMessage = error.ExceptionMessage ==null? error["error_description"].ToString():error.ExceptionMessage;

                Console.WriteLine(response.StatusCode);
                Console.WriteLine(this.ExceptionMessage);
                return null;
            }
            return await response.Content.ReadAsAsync<TokenResponse>();
        }

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <returns></returns>
         public async Task<TokenResponse> RefreshToken()
         {
             if (this.CurrentToken == null)
                 throw new Exception("请先调用GetToken 方法获取令牌，然后设置");
             return await GetToken("refresh_token", this.CurrentToken.RefreshToken);
         }

         public async Task<TokenResponse> RefreshToken(TokenResponse token)
         {
             this.CurrentToken = token;
             return await RefreshToken();
         }

         /// <summary>
         /// 使用之前的访问令牌，刷新令牌，返回一个携带新令牌的资源访问客户端对象。注意，刷新令牌，请从CurrentToken 属性获取新令牌
         /// </summary>
         /// <param name="token"></param>
         /// <returns></returns>
         public async Task<HttpClient> GetResourceClient(TokenResponse token)
         {
             this.CurrentToken = token;
             var newToken = await RefreshToken();
             SetAuthorizationRequest(this.ResourceServerClient, newToken);
             this.CurrentToken = newToken;
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
       
    }
}
