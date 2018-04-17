using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.OAuth2.Mvc.Models
{
    public class LoginResultModel:UserModel
    {
        public string ErrorMessage { get; set; }
    }
}