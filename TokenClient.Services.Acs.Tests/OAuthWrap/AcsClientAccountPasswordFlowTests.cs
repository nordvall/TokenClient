using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenClient.Common;
using TokenClient.Protocols.OAuthWrap;
using TokenClient.Services.Acs.OAuthWrap;

namespace TokenClient.Services.Acs.Tests
{
    [TestFixture]
    public class AcsClientAccountPasswordFlowTests
    {
        private readonly ClientAccountPasswordTokenRequest _tokenRequest;
        private readonly Uri _serviceUri;

        public AcsClientAccountPasswordFlowTests()
        {
            _tokenRequest = new ClientAccountPasswordTokenRequest("myClient", "mySecret", "myScope");
            _serviceUri = new Uri("https://acs.example.com");
        }

        [Test]
        public void RequestAccessToken_RequestUrlIsCorrect()
        {
            ProtocolResponse oauthResponse = CreateSwtTokenResponse();
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();

            ProtocolRequest receivedRequest = null;
            httpAdapter.SendRequest(Arg.Do<ProtocolRequest>(request => receivedRequest = request))
                .Returns(oauthResponse);

            var flow = new AcsClientAccountPasswordFlow(_serviceUri, _tokenRequest, httpAdapter);

            // Act
            flow.RequestAccessToken();

            // Assert
            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Uri expectedUri = new Uri(_serviceUri, "/WRAPv0.9/");

            Assert.AreEqual(expectedUri, receivedRequest.Url.BuildUri());
        }

        [Test]
        public void RequestAccessToken_RequestContentIsCorrect()
        {
            ProtocolResponse oauthResponse = CreateSwtTokenResponse();
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();
            
            ProtocolRequest receivedRequest = null;
            httpAdapter.SendRequest(Arg.Do<ProtocolRequest>(request => receivedRequest = request))
                .Returns(oauthResponse);

            var flow = new AcsClientAccountPasswordFlow(_serviceUri, _tokenRequest, httpAdapter);

            flow.RequestAccessToken();

            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Assert.AreEqual(receivedRequest.BodyParameters["wrap_name"], _tokenRequest.ClientId);
            Assert.AreEqual(receivedRequest.BodyParameters["wrap_password"], _tokenRequest.ClientSecret);
            Assert.AreEqual(receivedRequest.BodyParameters["wrap_scope"], _tokenRequest.Scope);
        }

        [Test]
        public void RequestAccessToken_ReceivedTokenIsCorrect()
        {
            ProtocolResponse oauthResponse = CreateSwtTokenResponse();
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();
            httpAdapter.SendRequest(Arg.Any<ProtocolRequest>()).Returns(oauthResponse);

            var flow = new AcsClientAccountPasswordFlow(_serviceUri, _tokenRequest, httpAdapter);

            SwtSecurityToken receivedToken = flow.RequestAccessToken() as SwtSecurityToken;

            Assert.AreEqual("myScope", receivedToken.Audience);
            Assert.AreEqual("http://mysts", receivedToken.Issuer);
            Assert.Greater(DateTime.UtcNow, receivedToken.ValidTo);
        }

        private ProtocolResponse CreateSwtTokenResponse()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Issuer={0}&", HttpUtility.UrlEncode("http://mysts"));
            builder.AppendFormat("Audience={0}", HttpUtility.UrlEncode("myScope"));

            
            var bodyParameters = new Dictionary<string,string>
            {
                {"wrap_access_token", builder.ToString()},
                {"wrap_access_token_expires_in", "900"}
            };

            var oauthResponse = new ProtocolResponse(bodyParameters);

            return oauthResponse;
        }

        private HttpResponseMessage CreateErrorResponse(HttpStatusCode httpCode, string errorCode, string errorMessage)
        {
            dynamic response = new
            {
                error = errorCode,
                error_description = errorMessage
            };

            string serializedResponse = JsonConvert.SerializeObject(response);

            var httpResponse = new HttpResponseMessage(httpCode)
            {
                Content = new StringContent(serializedResponse)
            };

            return httpResponse;
        }
    }
}
