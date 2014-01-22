using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuthWrap
{
    public class RequestParameters
    {
        /// <summary>
        /// The requested access level
        /// </summary>
        public string Scope { get; private set; }

        /// <summary>
        /// The target resource
        /// </summary>
        public string Resource { get; private set; }
    }
}
