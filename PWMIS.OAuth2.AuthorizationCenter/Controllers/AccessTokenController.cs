using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PWMIS.OAuth2.AuthorizationCenter.Controllers
{
    public class AccessTokenController : ApiController
    {
        // GET api/<controller>
         [Authorize]
        public IEnumerable<string> Get()
        {
            //此接口将返回用户的身份等信息:
            return new string[] { User.Identity.Name, User.Identity.IsAuthenticated.ToString() ,User.Identity.AuthenticationType };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}