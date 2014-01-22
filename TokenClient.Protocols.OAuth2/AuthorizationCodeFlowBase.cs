using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenClient.Common;
using TokenClient.Protocols.OAuth2.Http;

namespace TokenClient.Protocols.OAuth2
{
    public abstract class AuthorizationCodeFlowBase
    {
        protected string _accessCode;
        private readonly string _state;
        protected readonly ClientCredentials _clientCredentials;
        protected readonly RequestParameters _parameters;
        protected readonly Uri _serviceUri;
        protected readonly IHttpClient _httpClient;

        public AuthorizationCodeFlowBase(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters)
            : this(serviceUri, clientCredentials, parameters, new OAuthHttpClient())
        {

        }

        public AuthorizationCodeFlowBase(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters, IHttpClient httpClient)
        {
            _serviceUri = serviceUri;
            _clientCredentials = clientCredentials;
            _parameters = parameters;
            _httpClient = httpClient;
            _state = Guid.NewGuid().ToString("N");
        }

        public string FlowId 
        { 
            get { return _state; } 
        }

        public Uri GetAuthorizationUri()
        {
            Dictionary<string, string> parameters = GetAuthorizationRequestParameters();

            var urlParts = new UrlParts(AuthorizationEndpoint, parameters);
            Uri uri = urlParts.BuildUri();
            
            return uri;
        }

        protected virtual Uri AuthorizationEndpoint
        {
            get { return new Uri(_serviceUri, "authorize"); }
        }

        protected virtual Dictionary<string,string> GetAuthorizationRequestParameters()
        {
            var parameters = new Dictionary<string, string>(5);
            parameters.Add("response_type", "code");
            parameters.Add("client_id", _clientCredentials.ClientId);
            parameters.Add("redirect_uri", _parameters.RedirectUri.ToString());
            parameters.Add("state", FlowId);
            parameters.Add("scope", _parameters.Scope);

            return parameters;
        }

        public void SetAccessCodeRepsonse(Uri resultUrl)
        {
            var urlParts = new UrlParts(resultUrl);

            string state = GetStateValueFromParameters(urlParts.QueryParameters);
            VerifyStateParameter(state);

            string code = GetAuthorizationCodeFromParameters(urlParts.QueryParameters);

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("No access code found");
            }

            _accessCode = code;
        }

        protected virtual string GetAuthorizationCodeFromParameters(Dictionary<string,string> parameters)
        {
            return parameters["code"];
        }

        protected virtual string GetStateValueFromParameters(Dictionary<string, string> parameters)
        {
            return parameters["state"];
        }

        protected virtual void VerifyStateParameter(string state)
        {
            if (!state.Equals(FlowId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Response does not belong to this flow.");
            }
        }

        public SecurityToken GetAccessToken()
        {
            Dictionary<string, string> parameters = CreateAccessTokenRequestParameters();
            ProtocolRequest oauthRequest = CreateProtocolRequest(parameters);
            ProtocolResponse oauthResponse = _httpClient.SendRequest(oauthRequest);
            SecurityToken token = CreateSecurityToken(oauthResponse);

            return token;
        }

        protected virtual Dictionary<string, string> CreateAccessTokenRequestParameters()
        {
            var formParameters = new Dictionary<string, string>(4)
            {
                {"response_type", "authorization_code"},
                {"client_id", _clientCredentials.ClientId},
                {"redirect_uri", _parameters.RedirectUri.ToString()},
                {"code", _accessCode}
            };

            return formParameters;
        }

        protected virtual Uri TokenRequestEndpoint
        {
            get { return new Uri(_serviceUri, "token"); }
        }

        protected virtual SecurityToken CreateSecurityToken(ProtocolResponse oauthResponse)
        {
            string tokenType = oauthResponse.BodyParameters["token_type"];
            string accessTokenString = oauthResponse.BodyParameters["access_token"];

            var token = new JwtSecurityToken(accessTokenString);
            return token;
        }

        protected virtual ProtocolRequest CreateProtocolRequest(Dictionary<string, string> parameters)
        {
            var oauthRequest = new ProtocolRequest()
            {
                BodyParameters = parameters,
                Method = HttpMethod.Post,
                Url = new UrlParts(TokenRequestEndpoint)
            };

            return oauthRequest;
        }
    }
}
