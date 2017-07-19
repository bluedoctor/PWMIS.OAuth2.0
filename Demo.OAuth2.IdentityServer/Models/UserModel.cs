using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.OAuth2.IdentityServer.Models
{
    public class UserModel
    {
        public string ID { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
        public string Roles { get; set; }
    }
}