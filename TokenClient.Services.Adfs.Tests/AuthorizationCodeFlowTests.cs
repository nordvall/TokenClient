using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Adfs.Tests
{
    [TestFixture]
    public class AuthorizationCodeFlowTests
    {
        private readonly ClientCredentials _credentials;
        private readonly AdfsOAuth2Service _service;
        private readonly RequestParameters _requestParameters;
        private readonly Uri _serviceUri;

        public AuthorizationCodeFlowTests()
        {
            _requestParameters = new RequestParameters(new Uri("https://www.myapplication.net"), "myResource", "myScope");
            _credentials = new ClientCredentials("myClient", "mySecret");
            _serviceUri = new Uri("https://adfs.example.com");
            _service = new AdfsOAuth2Service(_serviceUri);
        }

        [Test]
        public void GetAuthorizationUrl_UrlIsCorrect()
        {
            var flow = new AuthorizationCodeFlow(_credentials, _requestParameters, _service);

            Uri authorizationUri = flow.GetAuthorizationUrl();

            Assert.AreEqual(_serviceUri.Scheme, authorizationUri.Scheme, "Url scheme is incorrect");
            Assert.AreEqual(_serviceUri.Host, authorizationUri.Host, "Host name is incorrect");
            Assert.AreEqual(_serviceUri.Port, authorizationUri.Port, "Port is incorrect");
            Assert.AreEqual("/adfs/oauth2/authorize", authorizationUri.AbsolutePath, "Url path is incorrect");

            NameValueCollection queryParameters = HttpUtility.ParseQueryString(authorizationUri.Query);

            Assert.AreEqual(_requestParameters.RedirectUri, new Uri(queryParameters["redirect_uri"]), "Redirect uri is incorrect");
            Assert.AreEqual("code", queryParameters["response_type"], "Response type is incorrect");
            Assert.AreEqual(_credentials.ClientId, queryParameters["client_id"], "Client id is incorrect");
            Assert.AreEqual(_requestParameters.RedirectUri, queryParameters["redirect_uri"], "Redirect uri is incorrect");
            Assert.AreEqual(_requestParameters.State, queryParameters["state"], "State is incorrect");
            Assert.AreEqual(_requestParameters.Resource, queryParameters["resource"], "Resource is incorrect");
            Assert.IsFalse(queryParameters.AllKeys.Contains("scope"), "Url should not include scope parameter (unsupported by ADFS).");
            
        }
    }
}
