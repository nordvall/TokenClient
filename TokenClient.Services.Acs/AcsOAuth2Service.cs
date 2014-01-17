using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.ACS
{
    public class AcsOAuth2Service : IClientCredentialsService, IAuthorizationCodeService
    {
        private Uri _baseUri;
        
        public AcsOAuth2Service(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri AuthorizationEndpoint
        {
            get
            {
                var builder = new UriBuilder(_baseUri);
                builder.Path += "/v2/oauth2-13/";
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

        public HttpContent CreateClientCredentialsAccessTokenRequest(ClientCredentials credentials, string scope)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
                {"client_id", credentials.ClientId},
                {"client_secret", credentials.ClientSecret},
                {"scope", scope}
            };

            return new FormUrlEncodedContent(parameters);
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
    }
}
