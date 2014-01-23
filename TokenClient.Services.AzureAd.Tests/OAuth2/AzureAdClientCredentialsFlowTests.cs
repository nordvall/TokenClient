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
using TokenClient.Services.AzureAd.OAuth2;

namespace TokenClient.Services.AzureAd.Tests
{
    [TestFixture]
    public class AzureAdClientCredentialsFlowTests
    {
        private readonly ClientCredentialsTokenRequest _tokenRequest;
        private readonly Uri _serviceUri;

        public AzureAdClientCredentialsFlowTests()
        {
            _tokenRequest = new ClientCredentialsTokenRequest("myClient", "mySecret", "myScope");
            _serviceUri = new Uri("https://acs.example.com");
        }

        [Test]
        public void RequestAccessToken_RequestUrlIsCorrect()
        {
            JwtSecurityToken token = CreateJwtToken();
            ProtocolResponse oauthResponse = CreateJwtTokenResponse(token);
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();

            // Return stub response and keep request for later inspection
            ProtocolRequest receivedRequest = null;
            httpAdapter.SendRequest(Arg.Do<ProtocolRequest>(request => receivedRequest = request))
                .Returns(oauthResponse);

            var flow = new AzureAdClientCredentialsFlow(_serviceUri, _tokenRequest, httpAdapter);

            // Act
            flow.RequestAccessToken();

            // Assert
            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Uri expectedUri = new Uri(_serviceUri, _serviceUri.AbsolutePath + "/oauth2/token");

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

            var flow = new AzureAdClientCredentialsFlow(_serviceUri, _tokenRequest, httpAdapter);

            flow.RequestAccessToken();

            httpAdapter.Received(1).SendRequest(Arg.Any<ProtocolRequest>());

            Assert.AreEqual(receivedRequest.BodyParameters["grant_type"], "client_credentials");
            Assert.AreEqual(receivedRequest.BodyParameters["client_id"], _tokenRequest.ClientId);
            Assert.AreEqual(receivedRequest.BodyParameters["client_secret"], _tokenRequest.ClientSecret);
            Assert.AreEqual(receivedRequest.BodyParameters["resource"], _tokenRequest.Scope);
        }

        [Test]
        public void RequestAccessToken_ReceivedTokenIsCorrect()
        {
            JwtSecurityToken preparedToken = CreateJwtToken();
            ProtocolResponse oauthResponse = CreateJwtTokenResponse(preparedToken);
            IHttpClient httpAdapter = Substitute.For<IHttpClient>();
            httpAdapter.SendRequest(Arg.Any<ProtocolRequest>()).Returns(oauthResponse);

            var flow = new AzureAdClientCredentialsFlow(_serviceUri, _tokenRequest, httpAdapter);

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
                _tokenRequest.Scope,
                null,
                new Lifetime(DateTime.Now, DateTime.Now.AddMinutes(10)));
            return token;
        }
    }
}
