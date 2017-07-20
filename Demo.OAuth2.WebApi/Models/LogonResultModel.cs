using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.OAuth2.WebApi.Models
{
    public class LogonResultModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LogonMessage { get; set; }
    }
}