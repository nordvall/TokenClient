using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class ClientCredentialsFlow
    {
        private IClientCredentialsService _service;
        protected HttpClient _webClient;
        private ClientCredentials _credentials;
        private string _scope;

        public ClientCredentialsFlow(ClientCredentials credentials, string scope, IClientCredentialsService service)
        {
            _credentials = credentials;
            _scope = scope;
            _service = service;

            _webClient = new HttpClient();
        }

        public JwtSecurityToken RequestAccessToken()
        {
            HttpContent content = _service.CreateClientCredentialsAccessTokenRequest(_credentials,_scope);
            HttpResponseMessage response = _webClient.PostAsync(_service.TokenEndpoint, content).Result;

            string oauthResponseString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AccessTokenErrorResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenErrorResponse>(oauthResponseString);
                
                throw new Exception(oauthResponse.Description);
            }
            else
            {
                AccessTokenResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(oauthResponseString);
                var token = new JwtSecurityToken(oauthResponse.AccessToken);

                return token;
            }
        }
    }
}
