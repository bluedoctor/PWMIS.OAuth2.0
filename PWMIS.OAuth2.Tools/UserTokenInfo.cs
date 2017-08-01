using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    public class UserTokenInfo
    {
        //System.Threading.AutoResetEvent autoReset = new System.Threading.AutoResetEvent(true);
        public AutoResetEvent TokenLock { get; private set; } //ManualResetEvent

        public string UserName { get;private set; }
        public TokenResponse Token { get; private set; }

        public UserTokenInfo(string userName,TokenResponse token)
        {
            this.UserName = userName;
            this.Token = token;
            this.FirstUseTime = DateTime.Now;
            this.TokenLock = new AutoResetEvent(true);
        }

        public int UseCount { get; private set; }

        public DateTime FirstUseTime { get; private set; }
        public DateTime LastUseTime { get; private set; }

        public void ResetToken(TokenResponse token)
        {
            this.FirstUseTime = DateTime.Now;
            this.UseCount = 0;
            this.Token = token;
        }

        public void BeginUse()
        {
            //autoReset.WaitOne();
            this.UseCount ++;
        }

        public void EndUse()
        {
            this.UseCount--;
            this.LastUseTime = DateTime.Now;
        }

        //public void Dispose()
        //{
        //    this.EndUse();
        //    //autoReset.Set();
        //}
    }
}
