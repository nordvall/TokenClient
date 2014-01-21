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
    public class ClientCredentialsFlowBase
    {
        protected readonly ClientCredentials _clientCredentials;
        protected readonly RequestParameters _parameters;
        protected readonly Uri _serviceUri;
        protected readonly IOAuthHttpAdapter _httpAdapter;

        public ClientCredentialsFlowBase(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters)
            : this(serviceUri, clientCredentials, parameters, new OAuthHttpAdapter())
        {

        }

        public ClientCredentialsFlowBase(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters, IOAuthHttpAdapter httpAdapter)
        {
            _serviceUri = serviceUri;
            _clientCredentials = clientCredentials;
            _parameters = parameters;
            _httpAdapter = httpAdapter;
        }

        protected virtual HttpMethod AccessTokenRequestMethod
        {
            get { return HttpMethod.Post; }
        }

        public SecurityToken RequestAccessToken()
        {
            Dictionary<string,string> bodyParameters = CreateAccessTokenRequestParameters();

            UrlParts url = new UrlParts(TokenEndpoint)
            {
                Path = TokenEndpoint.AbsolutePath
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(url, AccessTokenRequestMethod, bodyParameters);

            ProtocolResponse oauthResponse = _httpAdapter.SendRequest(oauthRequest);

            SecurityToken token = CreateSecurityToken(oauthResponse);

            return token;
        }

        protected virtual SecurityToken CreateSecurityToken(ProtocolResponse oauthResponse)
        {
            string tokenType = oauthResponse.BodyParameters["token_type"];
            string accessTokenString = oauthResponse.BodyParameters["access_token"];

            var token = new JwtSecurityToken(accessTokenString);
            return token;
        }

        protected virtual ProtocolRequest CreateProtocolRequest(UrlParts requestUrl, HttpMethod method, Dictionary<string,string> parameters)
        {
            var oauthRequest = new ProtocolRequest()
            {
                Url = requestUrl,
                Method = method,
                BodyParameters = parameters
            };

            return oauthRequest;
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
