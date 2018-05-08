using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// WebAPI 认证处理程序
    /// </summary>
    /// <remarks>
    /// 需要在 WebApiApplication.Application_Start() 方法中，增加下面一行代码：
    ///   GlobalConfiguration.Configuration.MessageHandlers.Add(new AuthenticationHandler());
    /// </remarks>
    public class AuthenticationHandler : DelegatingHandler 
    {
        HttpClient _httpClient;
        bool IsOne = true ;//是否使用单个HttpClient对象实例

        public AuthenticationHandler()
        {
            _httpClient = InnerGetClient();
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        private HttpClient GetClient()
        {
            return IsOne ? _httpClient : InnerGetClient();
        }

        private HttpClient InnerGetClient()
        {
            HttpClient client = new HttpClient();
            string Host_AuthCenter = System.Configuration.ConfigurationManager.AppSettings["OAuth2Server"];// "http://localhost:60186";
            client.BaseAddress = new Uri(Host_AuthCenter);
            return client;
        }
        /*
         * 【认证处理程序】处理过程：
         * 1，客户端使用之前从【授权服务器】申请的访问令牌，访问【资源服务器】；
         * 2，【资源服务器】加载【认证处理程序】
         * 3，【认证处理程序】将来自客户端的访问令牌，拿到【授权服务器】进行验证；
         * 4，【授权服务器】验证客户端的访问令牌有效，【认证处理程序】写入身份验证票据；
         * 5，【资源服务器】的受限资源（API）验证通过访问，返回结果给客户端。
         */

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null && request.Headers.Authorization.Parameter != null)
            {
                string token = request.Headers.Authorization.Parameter;
                try
                {
                    HttpClient client = GetClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/api/AccessToken");//.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string[] result = await response.Content.ReadAsAsync<string[]>();//.Result;
                        ClaimsIdentity identity = new ClaimsIdentity(result[2]);
                        identity.AddClaim(new Claim(ClaimTypes.Name, result[0]));
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        HttpContext.Current.User = principal;
                        //添加角色示例，更多信息，请参考 https://msdn.microsoft.com/zh-cn/library/5k850zwb(v=vs.80).aspx
                        //string[] userRoles = ((RolePrincipal)User).GetRoles();
                        //Roles.AddUserToRole("JoeWorden", "manager");
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                }
            }
          
            return await base.SendAsync(request, cancellationToken);
        }
    }
}