using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Demo.OAuth2.Port.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务  
            config.MessageHandlers.Add(new RequestHandler());
            // Web API 路由  
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }  
    }
}