using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TokenClient.Protocols.OAuth2
{
    public abstract class OAuth2Flow
    {
        protected readonly ClientCredentials _clientCredentials;
        protected readonly RequestParameters _parameters;
        private HttpClient _client;
        protected readonly Uri _serviceUri;

        public OAuth2Flow(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters)
        {
            _serviceUri = serviceUri;
            _clientCredentials = clientCredentials;
            _parameters = parameters;
        }
        

        protected virtual HttpClient HttpClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                }

                return _client;
            }
        }

        protected virtual AccessTokenResponse ParseAccessTokenResponse(HttpResponseMessage response)
        {
            string oauthResponseString = response.Content.ReadAsStringAsync().Result;
            AccessTokenResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(oauthResponseString);
            return oauthResponse;
        }

        protected string CreateQueryStringFromParameters(NameValueCollection parameters)
        {
            Func<string, string> combineKeyAndValue = (key) =>
            {
                string encodedKey = HttpUtility.UrlEncode(key);
                string encodedValue = HttpUtility.UrlEncode(parameters[key]);
                return string.Format("{0}={1}", encodedKey, encodedValue);
            };

            IEnumerable<string> keyValuePairs = parameters.AllKeys.Select(combineKeyAndValue);

            return string.Join("&", keyValuePairs);
        }

        protected virtual void ThrowIfErrorResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string oauthResponseString = response.Content.ReadAsStringAsync().Result;
                AccessTokenErrorResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenErrorResponse>(oauthResponseString);
                throw new Exception(oauthResponse.Description);
            }
        }

        protected virtual SecurityToken CreateSecurityToken(AccessTokenResponse oauthResponse)
        {
            var token = new JwtSecurityToken(oauthResponse.AccessToken);
            return token;
        }
    }
}
