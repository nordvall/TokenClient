using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public interface IClientCredentialsService
    {
        Uri TokenEndpoint { get; }
        HttpContent CreateClientCredentialsAccessTokenRequest(ClientCredentials credentials, RequestParameters parameters);
        void ValidateHttpResponse(HttpResponseMessage response);

        SecurityToken CreateAccessToken(AccessTokenResponse tokenResponse);
    }
}
