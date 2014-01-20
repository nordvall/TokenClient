using Newtonsoft.Json;
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
using TokenClient.Protocols.OAuth2;
using TokenClient.Services.Acs.OAuth2;

namespace TokenClient.Services.Acs.Tests
{
    [TestFixture]
    public class ClientCredentialsFlowTests
    {
        private readonly ClientCredentials _credentials;
        private readonly RequestParameters _requestParameters;
        private readonly Uri _serviceUri;

        public ClientCredentialsFlowTests()
        {
            _requestParameters = new RequestParameters(new Uri("https://www.myapplication.net"), "myResource", "myScope");
            _credentials = new ClientCredentials("myClient", "mySecret");
            _serviceUri = new Uri("https://acs.example.com");
        }

        [Test]
        public void RequestAccessToken_RequestUrlIsCorrect()
        {
            var flow = new TestableAcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters);

            JwtSecurityToken token = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(token);
            flow.HttpHandler = new FakeHttpHandler(response);
            
            flow.RequestAccessToken();

            HttpRequestMessage request = flow.HttpHandler.ReceivedRequests.Single();

            Assert.AreEqual(new Uri(_serviceUri, "/v2/oauth2-13/"), request.RequestUri);
        }

        [Test]
        public void RequestAccessToken_RequestContentIsCorrect()
        {
            var flow = new TestableAcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters);

            JwtSecurityToken token = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(token);
            flow.HttpHandler = new FakeHttpHandler(response);

            flow.RequestAccessToken();

            HttpRequestMessage request = flow.HttpHandler.ReceivedRequests.Single();
            string requestBody = request.Content.ReadAsStringAsync().Result;
            NameValueCollection requestValues = HttpUtility.ParseQueryString(requestBody);

            Assert.AreEqual(requestValues["grant_type"], "client_credentials");
            Assert.AreEqual(requestValues["client_id"], _credentials.ClientId);
            Assert.AreEqual(requestValues["client_secret"], _credentials.ClientSecret);
            Assert.AreEqual(requestValues["scope"], _requestParameters.Resource);
        }

        [Test]
        public void RequestAccessToken_ReceivedTokenIsCorrect()
        {
            var flow = new TestableAcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters);

            JwtSecurityToken preparedToken = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(preparedToken);
            flow.HttpHandler = new FakeHttpHandler(response);

            JwtSecurityToken receivedToken = flow.RequestAccessToken() as JwtSecurityToken;

            Assert.AreEqual(preparedToken.Audience, receivedToken.Audience);
            Assert.AreEqual(preparedToken.IssuedAt, receivedToken.IssuedAt);
            Assert.AreEqual(preparedToken.Issuer, receivedToken.Issuer);
            Assert.AreEqual(preparedToken.Expiration, receivedToken.Expiration);
        }

        [Test]
        public void RequestAccessToken_WhenServerReturnsBadRequest_ExceptionIsThrown()
        {
            string errorDescription = "The request is invalid.";

            var flow = new TestableAcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters);

            HttpResponseMessage response = CreateErrorResponse(HttpStatusCode.BadRequest, "invalid_request", errorDescription);
            flow.HttpHandler = new FakeHttpHandler(response);

            Exception ex = Assert.Throws<Exception>(() => flow.RequestAccessToken());

            Assert.AreEqual(errorDescription, ex.Message);
        }

        private HttpResponseMessage CreateJwtTokenResponse(JwtSecurityToken token)
        {
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);

            dynamic response = new
            {
                access_token = tokenString,
                token_type = "urn:ietf:params:oauth:token-type:jwt",
                expires_in = "900",
            };

            string serializedResponse = JsonConvert.SerializeObject(response);

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedResponse)
            };

            return httpResponse;
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

        private JwtSecurityToken CreateJwtToken()
        {
            var token = new JwtSecurityToken(
                _serviceUri.ToString(),
                _requestParameters.Resource,
                null,
                new Lifetime(DateTime.Now, DateTime.Now.AddMinutes(10)));
            return token;
        }
    }
}
