using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Models
{
    public class LoginResultModel
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
        public string ErrorMessage { get; set; }
    }
}