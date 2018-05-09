using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Demo.OAuth2.WebApi2.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            //string token = this.ControllerContext.Request.Headers.Authorization.Parameter;

            return new string[] { "Resource Server2:",this.User.Identity.Name, 
                this.User.Identity.IsAuthenticated.ToString(), 
                this.User.Identity.AuthenticationType,
                DateTime.Now.ToLongTimeString()};
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
