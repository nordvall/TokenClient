using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TokenClient.Services.Acs.Tests
{
    public class FakeHttpHandler : HttpMessageHandler
    {
        HttpResponseMessage _response;
        List<HttpRequestMessage> _requests;

        public FakeHttpHandler(HttpResponseMessage response)
        {
            _response = response;
            _requests = new List<HttpRequestMessage>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CloneAndSaveRequest(request);
            
            return _response;
        }

        private void CloneAndSaveRequest(HttpRequestMessage request)
        {
            HttpRequestMessage clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
            
            foreach (var item in request.Headers)
	        {
                clonedRequest.Headers.Add(item.Key, item.Value);
	        }

            string requestBody = request.Content.ReadAsStringAsync().Result;
            clonedRequest.Content = new StringContent(requestBody);
            
            _requests.Add(clonedRequest);
        }

        public IEnumerable<HttpRequestMessage> ReceivedRequests
        {
            get { return _requests; }
        }
    }
}
