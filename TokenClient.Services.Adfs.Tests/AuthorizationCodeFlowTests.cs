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
        private readonly AuthorizationCodeTokenRequest _tokenRequest;
        private readonly Uri _serviceUri;

        public AuthorizationCodeFlowTests()
        {
            _tokenRequest = new AuthorizationCodeTokenRequest("myClient", "mySecret", "myScope", new Uri("https://www.myapplication.net"));
            _serviceUri = new Uri("https://adfs.example.com");
        }

        [Test]
        public void GetAuthorizationUrl_UrlIsCorrect()
        {
            var flow = new AdfsAuthorizationCodeFlow(_serviceUri, _tokenRequest);

            Uri uri = flow.GetAuthorizationUri();

            Assert.AreEqual(_serviceUri.Scheme, uri.Scheme, "Url scheme is incorrect");
            Assert.AreEqual(_serviceUri.Host, uri.Host, "Host name is incorrect");
            Assert.AreEqual(_serviceUri.Port, uri.Port, "Port is incorrect");
            Assert.AreEqual("/adfs/oauth2", uri.AbsolutePath, "Url path is incorrect");

            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);

            Assert.AreEqual(_tokenRequest.RedirectUri, new Uri(queryParameters["redirect_uri"]), "Redirect uri is incorrect");
            Assert.AreEqual("code", queryParameters["response_type"], "Response type is incorrect");
            Assert.AreEqual(_tokenRequest.ClientId, queryParameters["client_id"], "Client id is incorrect");
            Assert.AreEqual(_tokenRequest.RedirectUri, queryParameters["redirect_uri"], "Redirect uri is incorrect");
            Assert.AreEqual(flow.FlowId, queryParameters["state"], "State is incorrect");
            Assert.AreEqual(_tokenRequest.Scope, queryParameters["resource"], "Resource is incorrect");
            Assert.IsFalse(queryParameters.AllKeys.Contains("scope"), "Url should not include scope parameter (unsupported by ADFS).");
            
        }
    }
}
