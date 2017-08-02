using PWMIS.OAuth2.Tools;
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
            config.MessageHandlers.Add(new ProxyRequestHandler());
            // Web API 路由  
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
             config.Routes.MapHttpRoute(
                name: "MyApi",
                routeTemplate: "api2/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
              );
             //可以将MVC的URL映射为WebAPI的
            /*
             config.Routes.MapHttpRoute(
                name: "Form",
                routeTemplate: "Form/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
              );
             config.Routes.MapHttpRoute(
               name: "Project",
               routeTemplate: "Project/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
             );
             */ 
        }  
    }
}