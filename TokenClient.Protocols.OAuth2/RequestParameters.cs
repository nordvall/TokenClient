using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class RequestParameters
    {
        public RequestParameters(Uri redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public RequestParameters(Uri redirectUri, string resource, string scope)
            : this(redirectUri)
        {
            Resource = resource;
            Scope = scope;
        }

        /// <summary>
        /// The requested access level
        /// </summary>
        public string Scope { get; private set; }

        /// <summary>
        /// The target resource
        /// </summary>
        public string Resource { get; private set; }

        public Uri RedirectUri { get; private set; }
    }
}
