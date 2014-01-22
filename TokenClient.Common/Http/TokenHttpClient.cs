using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Common.Http;

namespace TokenClient.Common.Http
{
    public class TokenHttpClient : IHttpClient
    {
        private HttpRequestAdapter _requestAdapter;
        private HttpResponseAdapter _responseAdapter;

        public TokenHttpClient(HttpRequestAdapter requestAdapter, HttpResponseAdapter responseAdapter)
        {
            _requestAdapter = requestAdapter;
            _responseAdapter = responseAdapter;
        }

        public ProtocolResponse SendRequest(ProtocolRequest protocolRequest)
        {
            HttpRequestMessage httpRequest = _requestAdapter.CreateHttpRequest(protocolRequest);
            
            var httpClient = new HttpClient();
            HttpResponseMessage httpResponse = httpClient.SendAsync(httpRequest).Result;
            
            ProtocolResponse oauthResponse = _responseAdapter.CreateProtocolResponse(httpResponse);

            return oauthResponse;
        }
    }
}
