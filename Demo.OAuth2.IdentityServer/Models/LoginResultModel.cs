using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.OAuth2.IdentityServer.Models
{
    public class LoginResultModel:UserModel
    {
        public string ErrorMessage { get; set; }
    }
}