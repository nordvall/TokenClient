using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TokenClient.Common.Http
{
    public class FormHttpResponseAdapter : HttpResponseAdapter
    {
        protected override void AddBodyParameters(HttpResponseMessage httpResponse, ProtocolResponse protocolResponse)
        {
            string oauthResponseString = httpResponse.Content.ReadAsStringAsync().Result;

            NameValueCollection parameters = HttpUtility.ParseQueryString(oauthResponseString);

            foreach (string key in parameters.AllKeys)
            {
                protocolResponse.BodyParameters.Add(key, parameters[key]);
            }
        }
    }
}
