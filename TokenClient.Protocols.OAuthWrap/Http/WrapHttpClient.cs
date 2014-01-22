using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common.Http;

namespace TokenClient.Protocols.OAuthWrap.Http
{
    public class WrapHttpClient : TokenHttpClient
    {
        public WrapHttpClient()
            : base(new FormHttpRequestAdapter(), new FormHttpResponseAdapter())
        {

        }
    }
}
