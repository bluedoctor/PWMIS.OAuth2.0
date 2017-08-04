/*参考资源：
 * http://beginor.github.io/2015/01/24/oauth2-server-with-owin.html
 * http://www.cnblogs.com/Hai--D/p/6187051.html
 * http://www.cnblogs.com/xishuai/p/aspnet-webapi-owin-oauth2.html
 * http://www.07net01.com/2016/12/1753733.html
 * http://www.cnblogs.com/zhyp/p/5513355.html
 * 
 * 在相关的项目，安装如下程序包：
Install-Package Microsoft.AspNet.WebApi.Owin 
Install-Package Microsoft.Owin.Host.SystemWeb 
Install-Package Microsoft.AspNet.Identity.Owin  
Install-Package Microsoft.Owin.Cors 
 * 注意：Microsoft.Owin.Cors  这使得OWIN支持跨域
 * 
 * WebAPI跨域访问的资料：
 * http://www.cnblogs.com/landeanfen/p/5177176.html
 * 
 * SQL LocalDB 无法登录问题
 * 请使用下面命令：
 * sqllocaldb i v11.0
 * 查看到命名管道实例，
 * 然后，用SSMS 连接上管道实例后，执行下面类似的语句：
 * create login [IIS APPPOOL\ASP.NET v4.0] from windows;
   exec sp_addsrvrolemember N'IIS APPPOOL\ASP.NET v4.0', sysadmin 
 * 详细请参考：http://www.cnblogs.com/xwgli/p/3435282.html
 * 
 */

using PWMIS.OAuth2.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.OAuth2.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyConfig config = new ProxyConfig();
            config.EnableRequestLog = true;
            config.RouteMaps.Add(new ProxyRouteMap() {  
                Prefix="/api/", 
                Host="localhost:62477",
                Match = ""
            });

            config.RouteMaps.Add(new ProxyRouteMap()
            {
                Prefix = "/api2/",
                Host = "localhost:8018",
                Match = "/api2/",
                Map = "/"
            });
            string configStr = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            Console.WriteLine(configStr);
            //
            try
            {
                DoTest();

            }
            catch (Exception ex)
            { 
            
            }
            Console.Read();
        }

        static async void DoTest()
        {
            //string host_webapi = "http://localhost:8018";
            string host_webapi = "http://localhost:62477";
            string host_AuthorizationCenter = "http://localhost:60186";
            OAuthClientTest test = new OAuthClientTest(host_webapi, host_AuthorizationCenter);

            Console.WriteLine("-----PWMIS OAuth2.0 测试--------");
            //Console.WriteLine("测试 OAuth2 客户端模式");
            Console.WriteLine("按任意键开始测试");
            Console.ReadLine();
            await test.OAuth_ClientCredentials_Test();

            Console.WriteLine();
            Console.WriteLine("测试 OAuth2 密码模式");
            string apiUrl = "/api/values";
            //string apiUrl = "/Account/TestAuthrization";
            await test.OAuth_Password_Test2(apiUrl);
            Console.WriteLine("-----测试全部完成-----------------");
        }
    }
}
