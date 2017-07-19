using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;

namespace Demo.OAuth2.WebApi
{
    public class AuthenticationHandler : DelegatingHandler 
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null && request.Headers.Authorization.Parameter != null)
            {
                string token = request.Headers.Authorization.Parameter;

                string Host_AuthCenter = "http://localhost:60186";
                HttpClient _httpClient = new HttpClient(); ;
                _httpClient.BaseAddress = new Uri(Host_AuthCenter);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = _httpClient.GetAsync("/api/AccessToken").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string[] result = response.Content.ReadAsAsync<string[]>().Result;
                    ClaimsIdentity identity = new ClaimsIdentity(result[2]);
                    identity.AddClaim(new Claim(ClaimTypes.Name, result[0]));
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    HttpContext.Current.User = principal;
                }
            }
            
          
            return base.SendAsync(request, cancellationToken);
        }
    }
}