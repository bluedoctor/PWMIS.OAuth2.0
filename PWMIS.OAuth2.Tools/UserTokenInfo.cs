using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    public class UserTokenInfo
    {
        public string UserName { get;private set; }
        public TokenResponse Token { get; private set; }

        public UserTokenInfo(string userName,TokenResponse token)
        {
            this.UserName = userName;
            this.Token = token;
            this.FirstUseTime = DateTime.Now;
        }

        public int UseCount { get; set; }

        public DateTime FirstUseTime { get; private set; }

        public void ResetToken(TokenResponse token)
        {
            this.FirstUseTime = DateTime.Now;
            this.UseCount = 0;
        }
    }
}
