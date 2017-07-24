using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Demo.OAuth2.WebApi.Controllers
{
    public class LogonController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public Models.LogonResultModel Post(Demo.OAuth2.WebApi.Models.LogonModel model)
        {
            Demo.OAuth2.WebApi.Models.LogonResultModel result = new Models.LogonResultModel();
            result.UserId = 1;
            result.UserName = model.UserName;
            result.LogonMessage = "OK";

            return result;
        }

        [Authorize]
        [HttpPost]
        [Route("api/Logon/ChangePassword")]
        public Models.LogonResultModel ChangePassword(Demo.OAuth2.WebApi.Models.LogonModel model)
        {
            Demo.OAuth2.WebApi.Models.LogonResultModel result = new Models.LogonResultModel();
            result.UserId = 1;
            result.UserName = model.UserName;
            result.LogonMessage = "OK";

            return result;
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