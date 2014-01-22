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
using TokenClient.Protocols.OAuth2;
using TokenClient.Services.Acs.OAuth2;

namespace TokenClient.Services.Acs.Tests
{
    [TestFixture]
    public class AcsClientCredentialsFlowTests
    {
        private readonly ClientCredentials _credentials;
        private readonly RequestParameters _requestParameters;
        private readonly Uri _serviceUri;

        public AcsClientCredentialsFlowTests()
        {
            _requestParameters = new RequestParameters(new Uri("https://www.myapplication.net"), "myResource", "myScope");
            _credentials = new ClientCredentials("myClient", "mySecret");
            _serviceUri = new Uri("https://acs.example.com");
        }

        [Test]
        public void RequestAccessToken_RequestUrlIsCorrect()
        {
            JwtSecurityToken token = CreateJwtToken();
            ProtocolResponse oauthResponse = CreateJwtTokenResponse(token);
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();

            ProtocolRequest receivedRequest = null;
            httpAdapter.SendRequest(Arg.Do<ProtocolRequest>(request => receivedRequest = request))
                .Returns(oauthResponse);

            var flow = new AcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters, httpAdapter);

            // Act
            flow.RequestAccessToken();

            // Assert
            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Uri expectedUri = new Uri(_serviceUri, "/v2/oauth2-13/");

            Assert.AreEqual(expectedUri, receivedRequest.Url.BuildUri());
        }

        [Test]
        public void RequestAccessToken_RequestContentIsCorrect()
        {
            JwtSecurityToken token = CreateJwtToken();
            ProtocolResponse oauthResponse = CreateJwtTokenResponse(token);
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();
            
            ProtocolRequest receivedRequest = null;
            httpAdapter.SendRequest(Arg.Do<ProtocolRequest>(request => receivedRequest = request))
                .Returns(oauthResponse);

            var flow = new AcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters, httpAdapter);

            flow.RequestAccessToken();

            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Assert.AreEqual(receivedRequest.BodyParameters["grant_type"], "client_credentials");
            Assert.AreEqual(receivedRequest.BodyParameters["client_id"], _credentials.ClientId);
            Assert.AreEqual(receivedRequest.BodyParameters["client_secret"], _credentials.ClientSecret);
            Assert.AreEqual(receivedRequest.BodyParameters["scope"], _requestParameters.Resource);
        }

        [Test]
        public void RequestAccessToken_ReceivedTokenIsCorrect()
        {
            JwtSecurityToken preparedToken = CreateJwtToken();
            ProtocolResponse oauthResponse = CreateJwtTokenResponse(preparedToken);
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();
            httpAdapter.SendRequest(Arg.Any<ProtocolRequest>()).Returns(oauthResponse);

            var flow = new AcsClientCredentialsFlow(_serviceUri, _credentials, _requestParameters, httpAdapter);

            JwtSecurityToken receivedToken = flow.RequestAccessToken() as JwtSecurityToken;

            Assert.AreEqual(preparedToken.Audience, receivedToken.Audience);
            Assert.AreEqual(preparedToken.IssuedAt, receivedToken.IssuedAt);
            Assert.AreEqual(preparedToken.Issuer, receivedToken.Issuer);
            Assert.AreEqual(preparedToken.Expiration, receivedToken.Expiration);
        }

        private ProtocolResponse CreateJwtTokenResponse(JwtSecurityToken token)
        {
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);

            var bodyParameters = new Dictionary<string,string>
            {
                {"access_token", tokenString},
                {"token_type", "urn:ietf:params:oauth:token-type:jwt"},
                {"expires_in", "900"}
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
