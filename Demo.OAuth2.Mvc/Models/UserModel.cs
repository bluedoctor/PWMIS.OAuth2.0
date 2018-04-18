using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.OAuth2.Mvc.Models
{
    public class UserModel
    {
        /// <summary>
        /// 用户标识或者会话标识
        /// </summary>
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string ValidationCode { get; set; }
    }
}