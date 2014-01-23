using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuth2.Http;

namespace TokenClient.Protocols.OAuth2
{
    public class ClientCredentialsFlowBase
    {
        protected readonly ClientCredentialsTokenRequest _tokenRequest;
        protected readonly Uri _serviceUri;
        protected readonly IHttpClient _httpClient;

        public ClientCredentialsFlowBase(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest)
            : this(serviceUri, tokenRequest, new OAuthHttpClient())
        {

        }

        public ClientCredentialsFlowBase(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest, IHttpClient httpClient)
        {
            _serviceUri = serviceUri;
            _tokenRequest = tokenRequest;
            _httpClient = httpClient;
        }

        public SecurityToken RequestAccessToken()
        {
            Dictionary<string,string> bodyParameters = CreateAccessTokenRequestParameters();

            UrlParts url = new UrlParts(TokenEndpoint)
            {
                Path = TokenEndpoint.AbsolutePath
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(url, bodyParameters);

            ProtocolResponse oauthResponse = _httpClient.SendRequest(oauthRequest);

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

        protected virtual ProtocolRequest CreateProtocolRequest(UrlParts requestUrl, Dictionary<string,string> parameters)
        {
            var oauthRequest = new ProtocolRequest()
            {
                Url = requestUrl,
                BodyParameters = parameters
            };

            return oauthRequest;
        }

        protected virtual Dictionary<string, string> CreateAccessTokenRequestParameters()
        {
            var formParameters = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
                {"client_id", _tokenRequest.ClientId},
                {"client_secret", _tokenRequest.ClientSecret},
                {"scope", _tokenRequest.Scope}
            };

            return formParameters;
        }

        protected virtual Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, "token"); }
        }
    }
}
