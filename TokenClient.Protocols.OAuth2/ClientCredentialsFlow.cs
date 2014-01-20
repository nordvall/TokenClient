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
    public class ClientCredentialsFlow : OAuth2Flow
    {
        private IClientCredentialsService _service;
        protected HttpClient _webClient;
        private ClientCredentials _credentials;
        private RequestParameters _parameters;

        public ClientCredentialsFlow(ClientCredentials credentials, RequestParameters parameters, IClientCredentialsService service)
            : this(credentials, parameters, service, new HttpClientHandler())
        {

        }

        public ClientCredentialsFlow(ClientCredentials credentials, RequestParameters parameters, IClientCredentialsService service, HttpMessageHandler httpHandler)
        {
            _credentials = credentials;
            _parameters = parameters;
            _service = service;

            _webClient = new HttpClient(httpHandler);
        }

        public SecurityToken RequestAccessToken()
        {
            HttpContent content = _service.CreateClientCredentialsAccessTokenRequest(_credentials, _parameters);
            HttpResponseMessage httpResponse = _webClient.PostAsync(_service.TokenEndpoint, content).Result;

            _service.ValidateHttpResponse(httpResponse);

            AccessTokenResponse tokenResponse = ParseAccessTokenResponse(httpResponse);

            SecurityToken token = _service.CreateAccessToken(tokenResponse);

            return token;
        }
    }
}
