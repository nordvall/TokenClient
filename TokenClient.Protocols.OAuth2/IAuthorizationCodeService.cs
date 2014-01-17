using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public interface IAuthorizationCodeService
    {
        Uri AuthorizationEndpoint { get; }
        Uri TokenEndpoint { get; }
        Uri CreateAuthorizationUrl(ClientCredentials clientCredentials, RequestParameters parameters);
        HttpContent CreateAccessTokenRequestWithAuthorizationCode(ClientCredentials credentials, RequestParameters parameters, string accessCode);
    }
}
