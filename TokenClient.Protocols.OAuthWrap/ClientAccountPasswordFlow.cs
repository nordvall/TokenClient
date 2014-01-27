using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenClient.Common;
using TokenClient.Common.Tokens;
using TokenClient.Protocols.OAuthWrap.Http;

namespace TokenClient.Protocols.OAuthWrap
{
    public class ClientAccountPasswordFlow
    {
        protected readonly ClientAccountPasswordTokenRequest _tokenRequest;
        protected readonly Uri _serviceUri;
        protected readonly IHttpClient _httpClient;
        private WebToken _cachedToken;

        public ClientAccountPasswordFlow(Uri serviceUri, ClientAccountPasswordTokenRequest tokenRequest)
            : this(serviceUri, tokenRequest, new WrapHttpClient())
        {

        }

        public ClientAccountPasswordFlow(Uri serviceUri, ClientAccountPasswordTokenRequest tokenRequest, IHttpClient httpClient)
        {
            _serviceUri = serviceUri;
            _tokenRequest = tokenRequest;
            _httpClient = httpClient;
        }

        public string GetAccessTokenAsString()
        {
            string tokenString = RequestAccessToken();
            return tokenString;
        }
        
        public SwtSecurityToken GetAccessToken()
        {
            string tokenString = RequestAccessToken();
            NameValueCollection tokenParts = HttpUtility.ParseQueryString(tokenString);
            var token = new SwtSecurityToken(tokenParts);

            return token;
        }

        protected string RequestAccessToken()
        {
            if (_cachedToken == null || _cachedToken.Expiration < DateTime.Now.AddMinutes(1))
            {
                Dictionary<string, string> bodyParameters = CreateAccessTokenRequestParameters();

                UrlParts url = new UrlParts(TokenEndpoint)
                {
                    Path = TokenEndpoint.AbsolutePath
                };

                ProtocolRequest oauthRequest = CreateProtocolRequest(url, bodyParameters);

                ProtocolResponse oauthResponse = _httpClient.SendRequest(oauthRequest);

                _cachedToken = ExtractSecurityTokenFromResponse(oauthResponse);
            }

            return _cachedToken.Token;
        }

        protected virtual WebToken ExtractSecurityTokenFromResponse(ProtocolResponse oauthResponse)
        {
            string lifetime = oauthResponse.BodyParameters["wrap_access_token_expires_in"];
            string accessTokenString = oauthResponse.BodyParameters["wrap_access_token"];

            var token = new WebToken(accessTokenString);
            token.Expiration.AddSeconds(int.Parse(lifetime));
            
            return token;
        }

        protected virtual ProtocolRequest CreateProtocolRequest(UrlParts requestUrl, Dictionary<string, string> parameters)
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
                {"wrap_scope", _tokenRequest.Scope},
                {"wrap_name", _tokenRequest.ClientId},
                {"wrap_password", _tokenRequest.ClientSecret}
            };

            return formParameters;
        }

        protected virtual Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, "token"); }
        }
    }
}
