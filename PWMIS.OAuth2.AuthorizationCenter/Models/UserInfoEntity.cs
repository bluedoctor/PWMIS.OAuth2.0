using PWMIS.DataMap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Models
{
    public class UserInfoEntity:EntityBase
    {
        public UserInfoEntity()
        {
            TableName = "UserInfo";
            IdentityName = "ID";
            PrimaryKeys.Add("ID");
        }

        public int ID
        {
            get { return getProperty<int>("ID"); }
            set { setProperty("ID", value); }
        }

        public string UserName
        {
            get { return getProperty<string>("UserName"); }
            set { setProperty("UserName", value, 50); }
        }

        public string Password
        {
            get { return getProperty<string>("Password"); }
            set { setProperty("Password", value, 100); }
        }
    }
}