using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using PWMIS.OAuth2.Tools;

[assembly: OwinStartup(typeof(Demo.OAuth2.WebApi.Startup))]
namespace Demo.OAuth2.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            //ConfigureOAuth(app);

            WebApiConfig.Register(config);
            config.MessageHandlers.Add(new AuthenticationHandler());
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}