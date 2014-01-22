using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuthWrap
{
    public class ClientCredentials
    {
        public ClientCredentials(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }
    }
}
