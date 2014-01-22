using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Common.Http
{
    public abstract class HttpRequestAdapter
    {
        public HttpRequestMessage CreateHttpRequest(ProtocolRequest protocolRequest)
        {
            Uri requestUri = protocolRequest.Url.BuildUri();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
            AddHttpRequestContent(protocolRequest, httpRequest);

            return httpRequest;
        }

        protected abstract void AddHttpRequestContent(ProtocolRequest protocolRequest, HttpRequestMessage httpRequest);
    }
}
