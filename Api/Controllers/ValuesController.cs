using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        HttpClient client;


        public ValuesController(TokenConfigurations settings)
        {

            client = new HttpClient();

        }


        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [AllowAnonymous]
        [HttpPost]
        public object Post(
            [FromBody] loginModel value,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {

            var payload = new Dictionary<string, string>
               {
                 {"login", "vferreira-contractor"},
                 {"password", "mc1@2019"},
                 { "application","bonfe" }
               };

            Account _obj = (Account)PostToken(payload);

            if (_obj.Authenticated && !string.IsNullOrEmpty(_obj.Token.ToString()))
            {

                ClaimsIdentity identity = new ClaimsIdentity(
               new GenericIdentity("vferreira-contractor", "Login"),
                  new[] {
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                         new Claim(JwtRegisteredClaimNames.UniqueName, "vferreira-contractor")
                }
               );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(600);

                var handler = new JwtSecurityTokenHandler();

                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });

                var token = handler.WriteToken(securityToken);

                GetToken(token);
              

                return new
                {
                    authenticated = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Falha ao autenticar"
                };
            }

        }

        public object PostToken(Dictionary<string, string> payload)
        {

            object invoiceRequests;

            using (var client = new HttpClient())
            {

                string baseUrl = "https://devweb-platform.mc1.com.br/accesscontrolapi/authentication/authenticate";
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                client.PostAsync(baseUrl, c).Result;

                string conteudo = response.Content.ReadAsStringAsync().Result;

                invoiceRequests = JsonConvert.DeserializeObject<Account>(conteudo);

                return invoiceRequests;


            }
        }

        public void GetToken(string _token)
        {

            string x = string.Empty;

            if (!string.IsNullOrWhiteSpace(_token))
            {

                client.BaseAddress = new Uri("https://localhost:5001/api/ApiAWS/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                HttpResponseMessage responseMessage = client.GetAsync("https://localhost:5001/api/ApiAWS/").Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<List<string>>(responseData);
                }
            }


        }


    }
}
