using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common.Http;

namespace TokenClient.Protocols.OAuth2.Http
{
    public class OAuthHttpClient : TokenHttpClient
    {
        public OAuthHttpClient()
            : base(new FormHttpRequestAdapter(), new OAuthJsonHttpResponseAdapter())
        {

        }
    }
}
