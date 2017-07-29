using PWMIS.OAuth2.Tools;
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

namespace Demo.OAuth2.ConsoleTest
{
    public class OAuthClientTest
    {
        private HttpClient _httpClient;
        private OAuthClient oAuthCenterClient;

        /// <summary>
        /// 授权中心访问地址
        /// </summary>
        public string Host_AuthCenter { get; private set; }
        /// <summary>
        /// 以要访问的目标WebAPI地址的基础地址和授权认证中心的基础地址初始化本类
        /// </summary>
        /// <param name="hostAddress">目标WebAPI地址的基础地址</param>
        /// <param name="hostOAuthCenterAddress">授权认证中心的基础地址</param>
        public OAuthClientTest(string hostAddress,string hostOAuthCenterAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(hostAddress);
            this.Host_AuthCenter = hostOAuthCenterAddress;
            oAuthCenterClient = new OAuthClient(hostOAuthCenterAddress);
        }

     


        public async Task OAuth_ClientCredentials_Test()
        {
            var tokenResponse = oAuthCenterClient.GetToken("client_credentials").Result; //获取 access_token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync("/api/values");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Console.WriteLine("10秒后测试刷新AccessToken...");

            Thread.Sleep(10000);

            var tokenResponseTwo = oAuthCenterClient.GetToken("refresh_token", tokenResponse.RefreshToken).Result;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync("/api/values");
            //Assert.Equal(HttpStatusCode.OK, responseTwo.StatusCode);
            Console.WriteLine("刷新AccessToken 并且访问资源服务器成功.");
        }


        public async Task OAuth_Password_Test(string apiUrl)
        {
            var tokenResponse = oAuthCenterClient.GetToken("password", null, "pwmis", "oath2").Result;
            oAuthCenterClient.CurrentToken = tokenResponse;
            //获取 access_token 后10秒内必须使用它，否则会过期，需要刷新后取得它再访问资源服务器
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync(apiUrl);//"/api/values"
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("10秒后测试刷新AccessToken...");

            //Thread.Sleep(10000);
            for (int i = 0; i < 12; i++)
            {
                Console.WriteLine("-------------{0}-------------",i);
                var tokenResponseTwo = oAuthCenterClient.RefreshToken().Result;
                if (tokenResponseTwo != null)
                {
                    Console.WriteLine("第{0}次刷新令牌成功。", i);
                    oAuthCenterClient.CurrentToken = tokenResponseTwo;
                    oAuthCenterClient.SetAuthorizationRequest(_httpClient, tokenResponseTwo);
                    var responseTwo = await _httpClient.GetAsync(apiUrl);
                    if (responseTwo != null)
                        Console.WriteLine("原令牌第{0}次刷新令牌，访问资源，结果：{1}",i, responseTwo.StatusCode);
                }
                Thread.Sleep(1000);
            }
            

            //Console.WriteLine("-------------2-------------");
            //var tokenResponseThrid = oAuthCenterClient.GetToken("refresh_token", tokenResponse.RefreshToken).Result;
            //if (tokenResponseThrid != null)
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseThrid.AccessToken);
            //    var responseThrid = await _httpClient.GetAsync(apiUrl);
            //    if (responseThrid != null)
            //        Console.WriteLine("原令牌第二次刷新令牌，访问资源，结果：{0}", responseThrid.StatusCode);
            //}

            Console.WriteLine("测试完成.");
        }


        public async Task OAuth_AuthorizationCode_Test()
        {
            var authorizationCode = oAuthCenterClient.GetAuthorizationCode().Result; //获取 authorization_code
            var tokenResponse = oAuthCenterClient.GetToken("authorization_code", null, null, null, authorizationCode).Result; //根据 authorization_code 获取 access_token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync("/api/values");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Thread.Sleep(10000);

            var tokenResponseTwo = oAuthCenterClient.GetToken("refresh_token", tokenResponse.RefreshToken).Result; //根据 refresh_token 获取 access_token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync("/api/values");
            //Assert.Equal(HttpStatusCode.OK, responseTwo.StatusCode);
        }


        public async Task OAuth_Implicit_Test()
        {
            var clientId = "xishuai";
            string targetUrl = string.Format("/authorize?response_type=token&client_id={0}&redirect_uri={1}",
                    clientId,
                    HttpUtility.UrlEncode("http://localhost:8001/api/access_token")
            );
            var tokenResponse = await _httpClient.GetAsync(targetUrl);
            //redirect_uri: http://localhost:8001/api/access_token#access_token=AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAfoPB4HZ0PUe-X6h0UUs2q42&token_type=bearer&expires_in=10
            var accessToken = "";//get form redirect_uri
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/values");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
