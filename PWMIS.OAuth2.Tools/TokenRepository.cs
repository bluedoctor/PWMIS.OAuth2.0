using PWMIS.OAuth2.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.Tools
{
    public class TokenRepository
    {
        /// <summary>
        /// 获取用户的访问令牌，如果不存在，会抛异常
        /// </summary>
        /// <returns></returns>
        public static TokenResponse GetUserToken()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated==false)
                throw new Exception("获取用户访问令牌但是用户未登录！");
            string token_key = "AccessToken_"+ identity.Name ;
            var obj= HttpContext.Current.Cache[token_key];
            if (obj == null)
                throw new Exception("未找到用户的访问令牌，key:"+ token_key);
            return (TokenResponse)obj;
        }

       /// <summary>
       /// 获取用户访问令牌，如果没有找到，不抛出异常
       /// </summary>
       /// <returns></returns>
        public static TokenResponse TryGetUserToken()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated == false)
                throw new Exception("获取用户访问令牌但是用户未登录！");
            string token_key = "AccessToken_" + identity.Name;
            var obj = HttpContext.Current.Cache[token_key];
            if (obj == null)
                return null;
            else
                return (TokenResponse)obj;
        }

        public static void SetUserToken(TokenResponse value)
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated == false)
                throw new Exception("获取用户访问令牌但是用户未登录！");
            string token_key = "AccessToken_" + identity.Name;
            HttpContext.Current.Cache[token_key] = value;
        }

        public static void SetUserToken(TokenResponse value,string iDentityName)
        {
            string token_key = "AccessToken_" + iDentityName;
            HttpContext.Current.Cache[token_key] = value;
        }
    }
}