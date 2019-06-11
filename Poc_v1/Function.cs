using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Poc_v1
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(SQSEvent evnt)
        {
            string text = string.Empty;
        
            foreach (var message in evnt.Records)
            {
                text+=ProcessMessageAsync(message);
            }

            return text;
        }


        private string ProcessMessageAsync(SQSEvent.SQSMessage message)
        {
            

            try
            {
                 return JsonConvert.DeserializeObject(message.Body).ToString();

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}
