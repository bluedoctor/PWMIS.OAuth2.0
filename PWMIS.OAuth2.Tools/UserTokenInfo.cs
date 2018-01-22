using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// 用户令牌使用信息
    /// </summary>
    public class UserTokenInfo
    {
        /// <summary>
        /// 获取用于线程同步的对象
        /// </summary>
        public object SyncObject { get; private set; }

        //System.Threading.AutoResetEvent autoReset = new System.Threading.AutoResetEvent(true);
        //public AutoResetEvent TokenLock { get; private set; } //ManualResetEvent
        /// <summary>
        /// 当前令牌的用户
        /// </summary>
        public string UserName { get;private set; }
        /// <summary>
        /// 获取当前使用的令牌
        /// </summary>
        public TokenResponse Token { get; private set; }

       /// <summary>
       /// 使用用户名和令牌初始化对象
       /// </summary>
       /// <param name="userName"></param>
       /// <param name="token"></param>
        public UserTokenInfo(string userName,TokenResponse token)
        {
            this.UserName = userName;
            this.Token = token;
            this.FirstUseTime = DateTime.Now;
            this.LastUseTime = DateTime.Now;
            //this.TokenLock = new AutoResetEvent(true);
            this.SyncObject = new object();
        }

        private int _useCount = 0;
        /// <summary>
        /// 获取此令牌的使用次数
        /// </summary>
        public int UseCount { get { return _useCount; } }
        /// <summary>
        /// 令牌的首次使用时间
        /// </summary>
        public DateTime FirstUseTime { get; private set; }
        /// <summary>
        /// 令牌的上次使用时间
        /// </summary>
        public DateTime LastUseTime { get; private set; }
        /// <summary>
        /// 重新设置令牌
        /// </summary>
        /// <param name="token"></param>
        public void ResetToken(TokenResponse token)
        {
            this.FirstUseTime = DateTime.Now;
            this.LastUseTime = DateTime.Now;
            this._useCount = 0;
            this.Token = token;
        }
        /// <summary>
        /// 开始使用
        /// </summary>
        public void BeginUse()
        {
            this.LastUseTime = DateTime.Now;
            System.Threading.Interlocked.Increment(ref this._useCount);
        }

        /// <summary>
        /// 结束使用
        /// </summary>
        public void EndUse()
        {
            System.Threading.Interlocked.Decrement(ref this._useCount);
        }

        //public void Dispose()
        //{
        //    this.EndUse();
        //    //autoReset.Set();
        //}
    }
}
