using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace TokenClient.Protocols.OAuth2
{
    public class UriAndMethod
    {
        public UriAndMethod(Uri uri, HttpMethod method)
        {
            Uri = uri;
            Method = method;
        }

        public Uri Uri { get; private set; }
        public HttpMethod Method { get; private set; }
    }
}
