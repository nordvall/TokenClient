using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class RequestParameters
    {
        private RequestParameters()
        {
            State = Guid.NewGuid().ToString("N");
        }

        public RequestParameters(Uri redirectUri)
            : this()
        {
            RedirectUri = redirectUri;
        }

        public RequestParameters(Uri redirectUri, string resource, string scope)
            : this(redirectUri)
        {
            Resource = resource;
            Scope = scope;
        }

        public string State { get; private set; }
        public string Scope { get; private set; }
        public string Resource { get; private set; }
        public Uri RedirectUri { get; private set; }
    }
}
