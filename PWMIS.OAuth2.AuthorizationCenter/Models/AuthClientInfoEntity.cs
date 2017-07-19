using PWMIS.DataMap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Models
{
    public class AuthClientInfoEntity:EntityBase
    {
        public AuthClientInfoEntity()
        {
            TableName = "AuthClient";
            PrimaryKeys.Add("ClientId");
        }
        public string ClientId 
        {
            get { return getProperty<string>("ClientId"); }
            set { setProperty("ClientId", value, 128); }
        }
        public string ClientSecret
        {
            get { return getProperty<string>("ClientSecret"); }
            set { setProperty("ClientSecret", value, 128); }
        }
        public string ClientHost
        {
            get { return getProperty<string>("ClientHost"); }
            set { setProperty("ClientHost", value, 100); }
        }
        public DateTime RegDate
        {
            get { return getProperty<DateTime>("RegDate"); }
            set { setProperty("RegDate", value); }
        }
    }
}