using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// 登录结果类
    /// </summary>
    public class LogonResultModel
    {
        /// <summary>
        /// 用户标识，可选
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录结果消息，正常情况为 OK
        /// </summary>
        public string LogonMessage { get; set; }
    }
}
