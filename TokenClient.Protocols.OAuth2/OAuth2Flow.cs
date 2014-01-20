using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public abstract class OAuth2Flow
    {
        protected AccessTokenResponse ParseAccessTokenResponse(HttpResponseMessage response)
        {
            string oauthResponseString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AccessTokenErrorResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenErrorResponse>(oauthResponseString);
                throw new Exception(oauthResponse.Description);
            }
            else
            {
                AccessTokenResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(oauthResponseString);
                return oauthResponse;
            }
        }
    }
}
