using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuthWrap;

namespace TokenClient.Services.Acs.OAuthWrap
{
    public class AcsClientAccountPasswordFlow : ClientAccountPasswordFlow
    {
        public AcsClientAccountPasswordFlow(Uri serviceUri, ClientAccountPasswordTokenRequest tokenRequest)
            : base(serviceUri, tokenRequest)
        {

        }

        public AcsClientAccountPasswordFlow(Uri serviceUri, ClientAccountPasswordTokenRequest tokenRequest, IHttpClient httpAdapter)
            : base(serviceUri, tokenRequest, httpAdapter)
        {

        }

        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuthWrapUrlPath); }
        }
    }
}
