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

namespace TokenClient.Services.Acs.Tests
{
    [TestFixture]
    public class ClientCredentialsFlowTests
    {
        private readonly ClientCredentials _credentials;
        private readonly AcsOAuth2Service _service;
        private readonly RequestParameters _requestParameters;
        private readonly Uri _serviceUri;

        public ClientCredentialsFlowTests()
        {
            _requestParameters = new RequestParameters(new Uri("https://www.myapplication.net"), "myResource", "myScope");
            _credentials = new ClientCredentials("myClient", "mySecret");
            _serviceUri = new Uri("https://acs.example.com");
            _service = new AcsOAuth2Service(_serviceUri);
        }

        [Test]
        public void RequestAccessToken_RequestUrlIsCorrect()
        {
            JwtSecurityToken token = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(token);
            var httpHandler = new FakeHttpHandler(response);
            
            var flow = new ClientCredentialsFlowBase(_credentials, _requestParameters, _service, httpHandler);
            flow.RequestAccessToken();

            HttpRequestMessage request = httpHandler.ReceivedRequests.Single();

            Assert.AreEqual(new Uri(_serviceUri, "/v2/oauth2-13/"), request.RequestUri);
        }

        [Test]
        public void RequestAccessToken_RequestContentIsCorrect()
        {
            JwtSecurityToken token = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(token);
            var httpHandler = new FakeHttpHandler(response);

            var flow = new ClientCredentialsFlowBase(_credentials, _requestParameters, _service, httpHandler);
            flow.RequestAccessToken();

            HttpRequestMessage request = httpHandler.ReceivedRequests.Single();
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
            JwtSecurityToken preparedToken = CreateJwtToken();
            HttpResponseMessage response = CreateJwtTokenResponse(preparedToken);
            var httpHandler = new FakeHttpHandler(response);

            var flow = new ClientCredentialsFlowBase(_credentials, _requestParameters, _service, httpHandler);
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

            HttpResponseMessage response = CreateErrorResponse(HttpStatusCode.BadRequest, "invalid_request", errorDescription);
            var httpHandler = new FakeHttpHandler(response);

            var flow = new ClientCredentialsFlowBase(_credentials, _requestParameters, _service, httpHandler);
            
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
