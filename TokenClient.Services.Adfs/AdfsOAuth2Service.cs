using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Adfs
{
    public class AdfsOAuth2Service : IAuthorizationCodeService
    {
        private Uri _baseUri;

        public AdfsOAuth2Service(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri AuthorizationEndpoint
        {
            get
            {
                var builder = new UriBuilder(_baseUri);
                builder.Path = "/adfs/oauth2/authorize";
                return builder.Uri;
            }
        }

        public Uri TokenEndpoint
        {
            get
            {
                var builder = new UriBuilder(_baseUri);
                builder.Path = "/adfs/oauth2/token";
                return builder.Uri;
            }
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
    }
}
