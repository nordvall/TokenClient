using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class OAuthHttpAdapter : IOAuthHttpAdapter
    {
        private HttpClient _client;

        public OAuthHttpAdapter()
            : this(new HttpClientHandler())
        {

        }

        public OAuthHttpAdapter(HttpMessageHandler handler)
        {
            _client = new HttpClient(handler);
        }

        public ProtocolResponse SendRequest(ProtocolRequest oauthRequest)
        {
            HttpRequestMessage httpRequest = CreateHttpRequest(oauthRequest);
            HttpResponseMessage httpResponse = _client.SendAsync(httpRequest).Result;
            
            ThrowIfErrorResponse(httpResponse);

            ProtocolResponse oauthResponse = CreateProtocolResponse(httpResponse);

            return oauthResponse;
        }

        protected virtual ProtocolResponse CreateProtocolResponse(HttpResponseMessage httpResponse)
        {
            string oauthResponseString = httpResponse.Content.ReadAsStringAsync().Result;
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(oauthResponseString);

            var oauthResponse = new ProtocolResponse(parameters);

            return oauthResponse;
        }

        protected virtual HttpRequestMessage CreateHttpRequest(ProtocolRequest request)
        {
            Uri requestUri = request.Url.BuildUri();
            
            var httpRequest = new HttpRequestMessage(request.Method, requestUri);
            httpRequest.Content = new FormUrlEncodedContent(request.BodyParameters);
            
            return httpRequest;
        }

        protected virtual void ThrowIfErrorResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string oauthResponseString = response.Content.ReadAsStringAsync().Result;
                AccessTokenErrorResponse oauthResponse = JsonConvert.DeserializeObject<AccessTokenErrorResponse>(oauthResponseString);
                throw new Exception(oauthResponse.Description);
            }
        }
    }
}
