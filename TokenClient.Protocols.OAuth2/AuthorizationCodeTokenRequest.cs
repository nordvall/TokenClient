using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class AuthorizationCodeTokenRequest
    {
        public AuthorizationCodeTokenRequest(string clientId, string clientSecret, string scope, Uri redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
            Scope = scope;
        }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public Uri RedirectUri { get; private set; }

        public string Scope { get; private set; }
    }
}
