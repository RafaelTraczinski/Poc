using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Model
{
  
    [Route("api/[controller]")]
    public class ApiAWSController : Controller
    {
        // GET: api/<controller>
        [Authorize("Bearer")]
        [HttpGet]
        public IEnumerable<string> Get()
        {

            Poc_v1.Function _function = new Poc_v1.Function();

            Dictionary<string, string> _pessoa = new Dictionary<string, string>
            { {"Nome","Rafael"},
                {"Idade","36" }
            };
           
            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                    {
                        new SQSEvent.SQSMessage
                        {
                            Body =  JsonConvert.SerializeObject(_pessoa)
                         }
                    }
            };

            return new string[] { _function.FunctionHandler(sqsEvent) };

            

        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
