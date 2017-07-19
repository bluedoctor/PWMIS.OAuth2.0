using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PWMIS.OAuth2.AuthorizationCenter.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            string name = User.Identity.Name;
            return new string[] { name, "IsAuthenticated", User.Identity.IsAuthenticated.ToString() };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
