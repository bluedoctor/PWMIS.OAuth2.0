using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Demo.OAuth2.WebApi
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (clientId != "SOD" || clientSecret != "http://www.pwmis.com/sqlmap")
            {
                context.SetError("invalid_client", "client or clientSecret is not valid");
                return;
            }
            context.Validated();
        }

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

            if (context.UserName != "SOD" || context.Password != "http://www.pwmis.com/sqlmap")
            {
                context.SetError("invalid_identity", "username or password is not valid");
                return;
            }

            var OAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            OAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            context.Validated(OAuthIdentity);
        }

        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            if (context.TokenRequest.IsAuthorizationCodeGrantType ||
                context.TokenRequest.IsRefreshTokenGrantType ||
                context.TokenRequest.IsResourceOwnerPasswordCredentialsGrantType)
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }
    }
}