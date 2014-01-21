using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TokenClient.Protocols.OAuth2.Tests
{
    [TestFixture]
    public class OAuthHttpAdapterTests
    {
        [Test]
        public void SendRequest_WhenInvokedWithProtocolRequest_HttpRequestIsCorrect()
        {
            var requestUri = new Uri("https://sts.example.com/token");

            var requestParameters = new Dictionary<string, string>()
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(HttpMethod.Post, requestUri, requestParameters);

            var responseParameters = new Dictionary<string, string>()
            {
                { "attr1", "value1"},
                { "attr2", "value2"}
            };

            HttpResponseMessage httpResponse = CreateJsonHttpResponse(HttpStatusCode.OK, responseParameters);

            var httpHandler = new FakeHttpHandler(httpResponse);
            var httpAdapter = new OAuthHttpAdapter(httpHandler);

            httpAdapter.SendRequest(oauthRequest);

            HttpRequestMessage receivedRequest = httpHandler.ReceivedRequests.First();

            Assert.AreEqual(HttpMethod.Post, receivedRequest.Method);
            Assert.AreEqual(requestUri, receivedRequest.RequestUri);

            string requestString = receivedRequest.Content.ReadAsStringAsync().Result;
            NameValueCollection requestValues = HttpUtility.ParseQueryString(requestString);

            Assert.AreEqual("value1", requestValues["param1"]);
            Assert.AreEqual("value2", requestValues["param2"]);
        }

        [Test]
        public void SendRequest_WhenHttpErrorIsReceived_ExceptionIsThrown()
        {
            var requestUri = new Uri("https://sts.example.com/token");

            var requestParameters = new Dictionary<string, string>()
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(HttpMethod.Post, requestUri, requestParameters);

            var responseParameters = new Dictionary<string, string>()
            {
                { "error", "value1"},
                { "error_description", "value2"}
            };

            HttpResponseMessage httpResponse = CreateJsonHttpResponse(HttpStatusCode.BadRequest, responseParameters);

            var httpHandler = new FakeHttpHandler(httpResponse);
            var httpAdapter = new OAuthHttpAdapter(httpHandler);

            Exception receivedException = Assert.Throws<Exception>(() => httpAdapter.SendRequest(oauthRequest));
            Assert.AreEqual(responseParameters["error_description"], receivedException.Message);
        }

        [Test]
        public void SendRequest_WhenServerReplies_ProtocolResponseIsCorrect()
        {
            var requestUri = new Uri("https://sts.example.com/token");

            var requestParameters = new Dictionary<string, string>()
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(HttpMethod.Post, requestUri, requestParameters);

            var responseParameters = new Dictionary<string, string>()
            {
                { "attr1", "value1"},
                { "attr2", "value2"}
            };

            HttpResponseMessage httpResponse = CreateJsonHttpResponse(HttpStatusCode.OK, responseParameters);

            var httpHandler = new FakeHttpHandler(httpResponse);
            var httpAdapter = new OAuthHttpAdapter(httpHandler);

            ProtocolResponse receivedResponse = httpAdapter.SendRequest(oauthRequest);

            Assert.AreEqual("value1", receivedResponse.BodyParameters["attr1"]);
            Assert.AreEqual("value2", receivedResponse.BodyParameters["attr2"]);
        }

        private static ProtocolRequest CreateProtocolRequest(HttpMethod method, Uri requestUri, Dictionary<string,string> parameters)
        {
            var oauthRequest = new ProtocolRequest()
            {
                Url = new UrlParts(requestUri),
                Method = method
            };

            foreach (string key in parameters.Keys)
            {
                oauthRequest.BodyParameters.Add(key, parameters[key]);
            }

            return oauthRequest;
        }

        private static HttpResponseMessage CreateJsonHttpResponse(HttpStatusCode code, Dictionary<string, string> returnObject)
        {
            string jsonresponse = JsonConvert.SerializeObject(returnObject);

            var httpResponse = new HttpResponseMessage(code)
            {
                Content = new StringContent(jsonresponse)
            };

            return httpResponse;
        }
    }
}
