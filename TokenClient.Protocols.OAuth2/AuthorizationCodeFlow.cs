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
    public class AuthorizationCodeFlow : OAuth2Flow
    {
        private IAuthorizationCodeService _service;
        private HttpClient _webClient;
        private ClientCredentials _clientCredentials;
        private string _accessCode;
        private RequestParameters _parameters;

        public AuthorizationCodeFlow(ClientCredentials clientCredentials, RequestParameters parameters, IAuthorizationCodeService service)
            : this(clientCredentials, parameters, service, new HttpClientHandler())
        {
            
        }

        public AuthorizationCodeFlow(ClientCredentials clientCredentials, RequestParameters parameters, IAuthorizationCodeService service, HttpMessageHandler httpHandler)
        {
            _clientCredentials = clientCredentials;
            _webClient = new HttpClient(httpHandler);
            _parameters = parameters;
            _service = service;
        }

        public string FlowId 
        { 
            get { return _parameters.State; } 
        }

        public Uri GetAuthorizationUrl()
        {
            Uri authorizeUri = _service.CreateAuthorizationUrl(_clientCredentials, _parameters);

            return authorizeUri;
        }

        public void SetAccessCodeRepsonse(Uri resultUrl)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(resultUrl.Query);

            string state = parameters["state"];
            //VerifyStateParameter(state);

            string code = parameters["code"];

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("No access code found");
            }

            _accessCode = code;
        }

        private void VerifyStateParameter(string state)
        {
            if (!state.Equals(FlowId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Response does not belong to this flow.");
            }
        }

        public JwtSecurityToken GetAccessToken()
        {
            HttpContent content = _service.CreateAccessTokenRequestWithAuthorizationCode(_clientCredentials, _parameters, _accessCode);
            HttpResponseMessage response = _webClient.PostAsync(_service.TokenEndpoint, content).Result;

            AccessTokenResponse tokenResponse = ParseAccessTokenResponse(response);
            var token = new JwtSecurityToken(tokenResponse.AccessToken);

            return token;
        }
    }
}
