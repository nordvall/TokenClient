using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Acs
{
    public class AcsOAuth2Service : IClientCredentialsService, IAuthorizationCodeService
    {
        private Uri _baseUri;
        private const string _swtTokenType = "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";
        private const string _jwtTokenType = "urn:ietf:params:oauth:token-type:jwt";

        public AcsOAuth2Service(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri AuthorizationEndpoint
        {
            get
            {
                var builder = new UriBuilder(_baseUri);
                builder.Path += "v2/oauth2-13/";
                return builder.Uri;
            }
        }

        public Uri TokenEndpoint
        {
            get
            {
                var builder = new UriBuilder(_baseUri);
                builder.Path += "/v2/oauth2-13/";
                return builder.Uri;
            }
        }

        public HttpContent CreateClientCredentialsAccessTokenRequest(ClientCredentials credentials, RequestParameters parameters)
        {

            var formParameters = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
                {"client_id", credentials.ClientId},
                {"client_secret", credentials.ClientSecret},
                {"scope", parameters.Resource}
            };

            return new FormUrlEncodedContent(formParameters);
        }


        public HttpContent CreateAccessTokenRequestWithAuthorizationCode(ClientCredentials credentials, RequestParameters parameters, string accessCode)
        {
            var formParameters = new Dictionary<string, string>(4)
            {
                {"response_type", "authorization_code"},
                {"client_id", credentials.ClientId},
                {"redirect_uri", parameters.RedirectUri.ToString()},
                {"code", accessCode}
            };

            return new FormUrlEncodedContent(formParameters);
        }

        public Uri CreateAuthorizationUrl(ClientCredentials clientCredentials, RequestParameters parameters)
        {
            var urlBuilder = new UrlBuilder(AuthorizationEndpoint);
            urlBuilder.Parameters.Add("response_type", "code");
            urlBuilder.Parameters.Add("client_id", clientCredentials.ClientId);
            urlBuilder.Parameters.Add("redirect_uri", parameters.RedirectUri.ToString());
            urlBuilder.Parameters.Add("state", parameters.State);
            urlBuilder.Parameters.Add("resource", parameters.Resource);

            Uri authorizeUri = urlBuilder.Build();

            return authorizeUri;
        }


        public void ValidateHttpResponse(HttpResponseMessage response)
        {
            
        }


        public SecurityToken CreateAccessToken(AccessTokenResponse tokenResponse)
        {
            switch (tokenResponse.TokenType)
            {
                case _jwtTokenType:
                    return new JwtSecurityToken(tokenResponse.AccessToken);
                default:
                    throw new Exception("Unsupported token type");
            }
        }
    }
}
