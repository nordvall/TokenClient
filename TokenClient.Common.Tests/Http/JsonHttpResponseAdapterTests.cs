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
    public class JsonHttpResponseAdapterTests
    {
        [Test]
        public void CreateProtocolResponse_WhenHttpErrorIsReceived_ExceptionIsThrown()
        {
            var responseParameters = new Dictionary<string, string>()
            {
                { "error", "value1"},
                { "error_description", "value2"}
            };

            HttpResponseMessage httpResponse = CreateJsonHttpResponse(HttpStatusCode.BadRequest, responseParameters);

            var responseAdapter = new JsonHttpResponseAdapter();

            Assert.Throws<Exception>(() => responseAdapter.CreateProtocolResponse(httpResponse));
        }

        [Test]
        public void CreateProtocolResponse_WithCorrectHttpResponse_ProtocolResponseIsCorrect()
        {
            var responseParameters = new Dictionary<string, string>()
            {
                { "attr1", "value1"},
                { "attr2", "value2"}
            };

            HttpResponseMessage httpResponse = CreateJsonHttpResponse(HttpStatusCode.OK, responseParameters);

            var responseAdapter = new JsonHttpResponseAdapter();

            ProtocolResponse receivedResponse = responseAdapter.CreateProtocolResponse(httpResponse);

            Assert.AreEqual("value1", receivedResponse.BodyParameters["attr1"]);
            Assert.AreEqual("value2", receivedResponse.BodyParameters["attr2"]);
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
