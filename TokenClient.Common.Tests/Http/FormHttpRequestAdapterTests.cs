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
using TokenClient.Common;
using TokenClient.Common.Http;

namespace TokenClient.Common.Tests
{
    [TestFixture]
    public class FormHttpRequestAdapterTests
    {
        [Test]
        public void CreateHttpRequest_WhenInvokedWithProtocolRequest_HttpRequestIsCorrect()
        {
            var requestUri = new Uri("https://sts.example.com/token");

            var requestParameters = new Dictionary<string, string>()
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            ProtocolRequest oauthRequest = CreateProtocolRequest(HttpMethod.Post, requestUri, requestParameters);

            var requestAdapter = new FormHttpRequestAdapter();
            HttpRequestMessage httpRequest = requestAdapter.CreateHttpRequest(oauthRequest);

            Assert.AreEqual(HttpMethod.Post, httpRequest.Method);
            Assert.AreEqual(requestUri, httpRequest.RequestUri);

            string requestString = httpRequest.Content.ReadAsStringAsync().Result;
            NameValueCollection requestValues = HttpUtility.ParseQueryString(requestString);

            Assert.AreEqual("value1", requestValues["param1"]);
            Assert.AreEqual("value2", requestValues["param2"]);
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
    }
}
