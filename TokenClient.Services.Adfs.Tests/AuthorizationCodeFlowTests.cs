using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenClient.Protocols.OAuth2;
using TokenClient.Services.Adfs.OAuth2;

namespace TokenClient.Services.Adfs.Tests
{
    [TestFixture]
    public class AuthorizationCodeFlowTests
    {
        private readonly ClientCredentials _credentials;
        private readonly RequestParameters _requestParameters;
        private readonly Uri _serviceUri;

        public AuthorizationCodeFlowTests()
        {
            _requestParameters = new RequestParameters(new Uri("https://www.myapplication.net"), "myResource", "myScope");
            _credentials = new ClientCredentials("myClient", "mySecret");
            _serviceUri = new Uri("https://adfs.example.com");
        }

        [Test]
        public void GetAuthorizationUrl_UrlIsCorrect()
        {
            var flow = new AdfsAuthorizationCodeFlow(_serviceUri, _credentials, _requestParameters);

            UriAndMethod uri = flow.GetAuthorizationUriAndMethod();

            Assert.AreEqual(_serviceUri.Scheme, uri.Uri.Scheme, "Url scheme is incorrect");
            Assert.AreEqual(_serviceUri.Host, uri.Uri.Host, "Host name is incorrect");
            Assert.AreEqual(_serviceUri.Port, uri.Uri.Port, "Port is incorrect");
            Assert.AreEqual("/adfs/oauth2/authorize", uri.Uri.AbsolutePath, "Url path is incorrect");

            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Uri.Query);

            Assert.AreEqual(_requestParameters.RedirectUri, new Uri(queryParameters["redirect_uri"]), "Redirect uri is incorrect");
            Assert.AreEqual("code", queryParameters["response_type"], "Response type is incorrect");
            Assert.AreEqual(_credentials.ClientId, queryParameters["client_id"], "Client id is incorrect");
            Assert.AreEqual(_requestParameters.RedirectUri, queryParameters["redirect_uri"], "Redirect uri is incorrect");
            Assert.AreEqual(flow.FlowId, queryParameters["state"], "State is incorrect");
            Assert.AreEqual(_requestParameters.Resource, queryParameters["resource"], "Resource is incorrect");
            Assert.IsFalse(queryParameters.AllKeys.Contains("scope"), "Url should not include scope parameter (unsupported by ADFS).");
            
        }
    }
}
