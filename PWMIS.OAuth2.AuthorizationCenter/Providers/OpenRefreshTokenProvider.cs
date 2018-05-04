using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter
{
    public class OpenRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();
        private static Random ticketRandom = new Random();

        /// <summary>
        /// 生成 refresh_token
        /// </summary>
        public override void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            context.SetToken(DateTime.Now.ToString("HHmmssfff")+ ticketRandom.Next(100000,1000000) + Guid.NewGuid().ToString("n"));
            _refreshTokens[context.Token] = context.SerializeTicket();
        }


        /// <summary>
        /// 由 refresh_token 解析成 access_token
        /// </summary>
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_refreshTokens.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
    }
}