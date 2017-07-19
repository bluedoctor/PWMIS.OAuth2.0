using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using PWMIS.OAuth2.AuthorizationCenter.Models;
using PWMIS.OAuth2.AuthorizationCenter.Repository;
using PWMIS.OAuth2.AuthorizationCenter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter
{
    public class OpenAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 验证 client 信息
        /// </summary>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                context.SetError("invalid_client", "client or clientSecret is null or empty");
                return;
            }

            var identityRepository = IdentityRepositoryFactory.CreateInstance();
            try
            {
                if (!await identityRepository.ValidateClient(clientId, clientSecret))
                {
                    context.SetError("invalid_client", "client or clientSecret is not valid");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.SetError("identity_repository_error", ex.Message );
                Log("identity_repository_error:" + ex.Message);
                return;
            }
          
            context.Validated();
        }

        /// <summary>
        /// 生成 access_token（client credentials 授权方式）
        /// </summary>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(
                context.ClientId, OAuthDefaults.AuthenticationType),
                context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            context.Validated(identity);
        }

        /// <summary>
        /// 生成 access_token（resource owner password credentials 授权方式）
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (string.IsNullOrEmpty(context.UserName))
            {
                context.SetError("invalid_username", "username is not valid");
                return;
            }
            if (string.IsNullOrEmpty(context.Password))
            {
                context.SetError("invalid_password", "password is not valid");
                return;
            }

            IdentityService service = new IdentityService();
            try
            {
                LoginResultModel user = await service.UserLogin(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_identity", "username or password is not valid");
                    return;
                }
                else  if (string.IsNullOrEmpty(user.UserName))
                {
                    context.SetError("invalid_identity", user.ErrorMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                context.SetError("identity_service_error", ex.Message );
                Log("identity_service_error:" + ex.Message);
                return;
            }
           

            var OAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            OAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            context.Validated(OAuthIdentity);
        }

        /// <summary>
        /// 生成 authorization_code（authorization code 授权方式）、生成 access_token （implicit 授权模式）
        /// </summary>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            if (context.AuthorizeRequest.IsImplicitGrantType)
            {
                //implicit 授权方式
                var identity = new ClaimsIdentity("Bearer");
                context.OwinContext.Authentication.SignIn(identity);
                context.RequestCompleted();
            }
            else if (context.AuthorizeRequest.IsAuthorizationCodeGrantType)
            {
                //authorization code 授权方式
                var redirectUri = context.Request.Query["redirect_uri"];
                var clientId = context.Request.Query["client_id"];
                var identity = new ClaimsIdentity(new GenericIdentity(
                    clientId, OAuthDefaults.AuthenticationType));

                var authorizeCodeContext = new AuthenticationTokenCreateContext(
                    context.OwinContext,
                    context.Options.AuthorizationCodeFormat,
                    new AuthenticationTicket(
                        identity,
                        new AuthenticationProperties(new Dictionary<string, string>
                        {
                            {"client_id", clientId},
                            {"redirect_uri", redirectUri}
                        })
                        {
                            IssuedUtc = DateTimeOffset.UtcNow,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(context.Options.AuthorizationCodeExpireTimeSpan)
                        }));

                await context.Options.AuthorizationCodeProvider.CreateAsync(authorizeCodeContext);
                context.Response.Redirect(redirectUri + "?code=" + Uri.EscapeDataString(authorizeCodeContext.Token));
                context.RequestCompleted();
            }
        }

        /// <summary>
        /// 验证 authorization_code 的请求
        /// </summary>
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            var identityRepository = IdentityRepositoryFactory.CreateInstance();
            if ( await identityRepository.ExistsClientId( context.AuthorizeRequest.ClientId ) && 
                (context.AuthorizeRequest.IsAuthorizationCodeGrantType || context.AuthorizeRequest.IsImplicitGrantType))
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }

        /// <summary>
        /// 验证 redirect_uri
        /// </summary>
        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            context.Validated(context.RedirectUri);
        }

        /// <summary>
        /// 验证 access_token 的请求
        /// </summary>
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            if (context.TokenRequest.IsAuthorizationCodeGrantType || 
                context.TokenRequest.IsRefreshTokenGrantType || 
                context.TokenRequest.IsResourceOwnerPasswordCredentialsGrantType ||
                context.TokenRequest.IsClientCredentialsGrantType)
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }

        private void Log(string logText)
        {
            string logFile = System.Configuration.ConfigurationManager.AppSettings["LogFile"];
            if (string.IsNullOrEmpty(logFile))
                return;
            PWMIS.Core.CommonUtil.ReplaceWebRootPath(ref logFile);
            System.IO.File.AppendAllText(logFile, DateTime.Now.ToString()+ "--"+ logText+"\r\n");
        }
    }
}