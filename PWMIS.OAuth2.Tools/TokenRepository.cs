/*
 * 采用相对于用户的同步信号量，控制同一个用户在一段时间内对token的使用
 * 
 */ 

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
        public static UserTokenInfo GetUserToken()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated==false)
                throw new Exception("获取用户访问令牌但是用户未登录！");
            string token_key = "AccessToken_"+ identity.Name ;
            var obj= HttpContext.Current.Cache[token_key];
            if (obj == null)
                throw new Exception("未找到用户的访问令牌，key:"+ token_key);
            UserTokenInfo ut = (UserTokenInfo)obj;
            ut.UseCount++;
            return ut; 
        }

       /// <summary>
       /// 获取用户访问令牌，如果没有找到，不抛出异常
       /// </summary>
       /// <returns></returns>
        public static UserTokenInfo TryGetUserToken()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated == false)
                return null;
            string token_key = "AccessToken_" + identity.Name;
            var obj = HttpContext.Current.Cache[token_key];
            if (obj == null)
                return null;
           UserTokenInfo ut=(UserTokenInfo)obj;
           ut.UseCount++;
           return ut; 
        }

        public static void SetUserToken(TokenResponse value)
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity == null || identity.IsAuthenticated == false)
                throw new Exception("获取用户访问令牌但是用户未登录！");
            if (value == null)
                throw new ArgumentNullException();
            string token_key = "AccessToken_" + identity.Name;
            UserTokenInfo ut = new UserTokenInfo(identity.Name, value);
            HttpContext.Current.Cache[token_key] = ut;
        }

        public static void SetUserToken(TokenResponse value,string identityName)
        {
            if (value == null)
                throw new ArgumentNullException();
            string token_key = "AccessToken_" + identityName;
            UserTokenInfo ut = new UserTokenInfo(identityName, value);
            HttpContext.Current.Cache[token_key] = ut;
        }
    }
}