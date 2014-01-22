using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common.Http
{
    public class JsonHttpResponseAdapter : HttpResponseAdapter
    {
        protected override void AddBodyParameters(HttpResponseMessage httpResponse, ProtocolResponse protocolResponse)
        {
            string oauthResponseString = httpResponse.Content.ReadAsStringAsync().Result;
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(oauthResponseString);

            protocolResponse.BodyParameters.AddRange(parameters);
        }
    }
}
