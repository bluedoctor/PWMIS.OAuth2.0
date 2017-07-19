using PWMIS.Core.Extensions;
using PWMIS.DataMap.Entity;
using PWMIS.OAuth2.AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Repository
{
    public class AuthDbContext:DbContext
    {
        public AuthDbContext()
            : base("OAuth2")
        {
                    
        }


        protected override bool CheckAllTableExists()
        {
            base.CheckTableExists<AuthClientInfoEntity>();
            base.CheckTableExists<UserInfoEntity>();
            return true;
        }
    }
}