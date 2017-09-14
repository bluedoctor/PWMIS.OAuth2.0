using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    public class TokenManager:IDisposable
    {
        private static Dictionary<string, UserTokenInfo> dictUserToken = new Dictionary<string, UserTokenInfo>();
        private AutoResetEvent CurrentTokenLock;

        public string UserName { get; private set; }
        public UserTokenInfo CurrentUserTokenInfo { get; private set; }
        /// <summary>
        /// 获取刷新令牌之前的令牌
        /// </summary>
        public TokenResponse OldToken { get; private set; }

        public string TokenExctionMessage { get; private set; }

        /// <summary>
        /// 创建指定用户的令牌管理器
        /// </summary>
        /// <param name="userName"></param>
        public TokenManager(string userName)
        {
            this.UserName = userName;
        }

        /// <summary>
        /// 使用密码模式，给当前用户创建一个访问令牌
        /// </summary>
        /// <param name="password">用户登录密码</param>
        /// <returns></returns>
        public async Task<TokenResponse> CreateToken(string password)
        {
            OAuthClient oc = new OAuthClient();
            var tokenRsp= await oc.GetTokenOfPasswardGrantType(this.UserName, password);
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
        /// <returns></returns>
        public TokenResponse TakeToken()
        {
            if (dictUserToken.ContainsKey(this.UserName))
            {
                UserTokenInfo uti = dictUserToken[this.UserName];
                //获取当前用户的读写锁
                this.CurrentTokenLock = uti.TokenLock;
                //this.CurrentTokenLock.EnterUpgradeableReadLock();
                //Console.WriteLine("...EnterUpgradeableReadLock...,Thread ID:{0}",Thread.CurrentThread.ManagedThreadId);
                this.OldToken = uti.Token;
                //Thread.Sleep(1000);
                Console.WriteLine("thread waite one.thread:{0}", Thread.CurrentThread.ManagedThreadId);
                this.CurrentTokenLock.WaitOne(10000);

                if (DateTime.Now.Subtract(uti.FirstUseTime).TotalSeconds >= uti.Token.expires_in - 2)
                {
                    //等待所有使用此令牌的线程使用完成
                    Console.WriteLine("thread reset begin. thread:{0}", Thread.CurrentThread.ManagedThreadId);
                    this.CurrentTokenLock.Reset();
                    try
                    {
                        if(uti.UseCount>0)
                            Thread.Sleep(2000);//睡2秒，等待之前的请求处理完
                        //防止线程重入，再次判断
                        if (DateTime.Now.Subtract(uti.FirstUseTime).TotalSeconds >= uti.Token.expires_in - 2)
                        {
                            //刷新令牌
                            OAuthClient oc = new OAuthClient();
                            var newToken = oc.RefreshToken(uti.Token);
                            uti.ResetToken(newToken);
                            this.TokenExctionMessage = oc.ExceptionMessage;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.TokenExctionMessage = ex.Message;
                        return null;
                    }
                    finally
                    {
                        Console.WriteLine("thread set. thread:{0}", Thread.CurrentThread.ManagedThreadId);
                        this.CurrentTokenLock.Set();
                    }
                }
               
                Console.WriteLine("thread continue. thread:{0}", Thread.CurrentThread.ManagedThreadId);
                this.CurrentUserTokenInfo = uti;
                uti.BeginUse();
                this.CurrentTokenLock.Set();
                return uti.Token;
            }
            else
            {
                throw new Exception(this.UserName+" 还没有访问令牌。");
            }
        }

        public string ValidateToken(string token)
        {
            if (dictUserToken.ContainsKey(this.UserName))
            {
                var userToken = dictUserToken[this.UserName];
                return userToken.Token.AccessToken == token ?"OK":"TokenNotEqual";
            }
            return "NotExists";
        }

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
