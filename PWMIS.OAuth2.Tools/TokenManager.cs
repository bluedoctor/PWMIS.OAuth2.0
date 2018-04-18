using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// 令牌管理器
    /// </summary>
    public class TokenManager:IDisposable
    {
        private static Dictionary<string, UserTokenInfo> dictUserToken = new Dictionary<string, UserTokenInfo>();
        //private AutoResetEvent CurrentTokenLock;
        /// <summary>
        /// 获取令牌关联的用户名
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// 当前用户的令牌信息
        /// </summary>
        public UserTokenInfo CurrentUserTokenInfo { get; private set; }
        /// <summary>
        /// 获取刷新令牌之前的令牌
        /// </summary>
        public TokenResponse OldToken { get; private set; }
        /// <summary>
        /// 获取或者刷新令牌期间发生的错误
        /// </summary>
        public string TokenExctionMessage { get; private set; }
        /// <summary>
        /// 会话标识
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// 创建指定用户的令牌管理器
        /// </summary>
        /// <param name="userName">标识Token拥有者的唯一用户名，不可为空</param>
        /// <param name="sessionId">关联的会话标识，在创建令牌的时候可以使用，其它时候可以传空</param>
        public TokenManager(string userName,string sessionId)
        {
            this.UserName = userName;
            this.SessionID = sessionId;
        }

        /// <summary>
        /// 使用密码模式，给当前用户创建一个访问令牌
        /// </summary>
        /// <param name="password">用户登录密码</param>
        /// <param name="validationCode">验证码</param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateToken(string password,string validationCode=null)
        {
            OAuthClient oc = new OAuthClient();
            oc.SessionID = this.SessionID;
            var tokenRsp= await oc.GetTokenOfPasswardGrantType(this.UserName, password, validationCode);
            if (tokenRsp != null)
            {
                UserTokenInfo uti = new UserTokenInfo(this.UserName, tokenRsp);
                dictUserToken[this.UserName] = uti;
            }
            else
            {
                this.TokenExctionMessage = oc.ExceptionMessage;
            }
            return tokenRsp;
        }

        /// <summary>
        /// 取一个访问令牌
        /// </summary>
        /// <returns>如果没有或者获取令牌失败，返回空</returns>
        public TokenResponse TakeToken()
        {
            if (dictUserToken.ContainsKey(this.UserName))
            {
                UserTokenInfo uti = dictUserToken[this.UserName];
                //获取当前用户的读写锁
                //this.CurrentTokenLock = uti.TokenLock;
                //this.CurrentTokenLock.EnterUpgradeableReadLock();
                //Console.WriteLine("...EnterUpgradeableReadLock...,Thread ID:{0}",Thread.CurrentThread.ManagedThreadId);
                this.OldToken = uti.Token;

                //如果令牌超期，刷新令牌
                if (DateTime.Now.Subtract(uti.FirstUseTime).TotalSeconds >= uti.Token.expires_in )
                {
                    lock (uti.SyncObject)
                    {
                        //防止线程重入，再次判断
                        if (DateTime.Now.Subtract(uti.FirstUseTime).TotalSeconds >= uti.Token.expires_in)
                        {
                            //等待之前的用户使用完令牌
                            while (uti.UseCount > 0)
                            {
                                if (DateTime.Now.Subtract(uti.LastUseTime).TotalSeconds > 10)
                                {
                                    //如果发出请求超过10秒使用计数还大于0，可以认为资源服务器响应缓慢，最终请求此资源可能会拒绝访问
                                    this.TokenExctionMessage = "Resouce Server maybe Request TimeOut.";
                                    OAuthClient.WriteErrorLog("00", "**警告** "+DateTime.Now.ToString()+"：用户"+this.UserName+" 最近一次使用当前令牌（"
                                        +uti.Token.AccessToken +"）已经超时（10秒）,使用次数："+uti.UseCount+"。\r\n**下面将刷新令牌，但可能导致之前还未处理完的资源服务器访问被拒绝访问。");
                                    break;
                                }
                                System.Threading.Thread.Sleep(100);
                                Console.WriteLine("----waite token Use Count is 0.--------");
                            }
                            //刷新令牌
                            try
                            {
                                OAuthClient oc = new OAuthClient();
                                var newToken = oc.RefreshToken(uti.Token);
                                if (newToken == null)
                                    throw new Exception("Refresh Token Error:" + oc.ExceptionMessage);
                                else if( string.IsNullOrEmpty( newToken.AccessToken))
                                    throw new Exception("Refresh Token Error:Empty AccessToken. Other Message:" + oc.ExceptionMessage);

                                uti.ResetToken(newToken);
                                this.TokenExctionMessage = oc.ExceptionMessage;
                            }
                            catch (Exception ex)
                            {
                                this.TokenExctionMessage = ex.Message;
                                return null;
                            }
                           
                        }
                    }//end lock
                }
               
                this.CurrentUserTokenInfo = uti;
                uti.BeginUse();
                //this.CurrentTokenLock.Set();
                return uti.Token;
            }
            else
            {
                //throw new Exception(this.UserName+" 还没有访问令牌。");
                this.TokenExctionMessage = "UserNoToken";
                return null;
            }
        }

        /// <summary>
        /// 验证当令牌对当前用户是否有效，如果有效，返回 OK
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ValidateToken(string token)
        {
            if (dictUserToken.ContainsKey(this.UserName))
            {
                var userToken = dictUserToken[this.UserName];
                return userToken.Token.AccessToken == token ?"OK":"TokenNotEqual";
            }
            return "NotExists";
        }
        /// <summary>
        /// 释放当前令牌使用计数
        /// </summary>
        public void Dispose()
        {
            if (this.CurrentUserTokenInfo != null)
            {
                this.CurrentUserTokenInfo.EndUse();
                //不可以在这里释放读写锁，此时读写锁对象可能不在开始的线程，会引发 “可升级锁定未经保持即被释放”的异常
                //Console.WriteLine("...Try ExitUpgradeableReadLock...,Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);
                //this.CurrentTokenLock.ExitUpgradeableReadLock();
            }
        }
    }
}
