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

namespace TokenClient.Protocols.OAuth2
{
    public abstract class AuthorizationCodeFlowBase : OAuth2Flow
    {
        protected string _accessCode;
        private readonly string _state;

        public AuthorizationCodeFlowBase(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters)
            : base(serviceUri, clientCredentials, parameters)
        {
            _state = Guid.NewGuid().ToString("N");
        }

        public string FlowId 
        { 
            get { return _state; } 
        }

        public UriAndMethod GetAuthorizationUriAndMethod()
        {
            NameValueCollection parameters = GetAuthorizationRequestParameters();

            var urlBuilder = new UriBuilder(AuthorizationEndpoint);
            urlBuilder.Query = CreateQueryStringFromParameters(parameters);

            return new UriAndMethod(urlBuilder.Uri, AuthorizationRequestMethod);
        }

        protected virtual Uri AuthorizationEndpoint
        {
            get { return new Uri(_serviceUri, "authorize"); }
        }

        protected virtual NameValueCollection GetAuthorizationRequestParameters()
        {
            var parameters = new NameValueCollection(5);
            parameters.Add("response_type", "code");
            parameters.Add("client_id", _clientCredentials.ClientId);
            parameters.Add("redirect_uri", _parameters.RedirectUri.ToString());
            parameters.Add("state", FlowId);
            parameters.Add("scope", _parameters.Scope);

            return parameters;
        }

        protected virtual HttpMethod AuthorizationRequestMethod
        {
            get { return HttpMethod.Get; }
        }

        public void SetAccessCodeRepsonse(Uri resultUrl)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(resultUrl.Query);

            string state = GetStateValueFromParameters(parameters);
            VerifyStateParameter(state);

            string code = GetAuthorizationCodeFromParameters(parameters);

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("No access code found");
            }

            _accessCode = code;
        }

        protected virtual string GetAuthorizationCodeFromParameters(NameValueCollection parameters)
        {
            return parameters["code"];
        }

        protected virtual string GetStateValueFromParameters(NameValueCollection parameters)
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
            HttpContent content = new FormUrlEncodedContent(parameters);
            
            HttpResponseMessage response = HttpClient.PostAsync(TokenRequestEndpoint, content).Result;

            ThrowIfErrorResponse(response);

            AccessTokenResponse oauthResponse = ParseAccessTokenResponse(response);
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
    }
}
