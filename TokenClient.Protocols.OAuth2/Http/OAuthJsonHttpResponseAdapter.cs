using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common.Http;

namespace TokenClient.Protocols.OAuth2.Http
{
    public class OAuthJsonHttpResponseAdapter : JsonHttpResponseAdapter
    {
        protected override void ThrowIfErrorResponse(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                AccessTokenErrorResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenErrorResponse>(responseContent);
                throw new Exception(oauthResponse.Description);
            }
        }
    }
}
