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
    public class ClientCredentialsFlowBase : OAuth2Flow
    {
        public ClientCredentialsFlowBase(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }

        public SecurityToken RequestAccessToken()
        {
            Dictionary<string,string> parameters = CreateAccessTokenRequestParameters();
            HttpContent content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage httpResponse = HttpClient.PostAsync(TokenEndpoint, content).Result;

            ThrowIfErrorResponse(httpResponse);
            AccessTokenResponse tokenResponse = ParseAccessTokenResponse(httpResponse);
            SecurityToken token = CreateSecurityToken(tokenResponse);

            return token;
        }

        protected virtual Dictionary<string, string> CreateAccessTokenRequestParameters()
        {
            var formParameters = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
                {"client_id", _clientCredentials.ClientId},
                {"client_secret", _clientCredentials.ClientSecret},
                {"scope", _parameters.Scope}
            };

            return formParameters;
        }

        protected virtual Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, "token"); }
        }
    }
}
