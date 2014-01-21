using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TokenClient.Protocols.OAuth2.Tests
{
    [TestFixture]
    public class UrlPartsTests
    {
        [Test]
        public void Ctor_WhenUriIsProvided_AllPartsAreKept()
        {
            var testUri = new Uri("https://www.example.com:88/equipment/cars?year=1992&color=blue");

            var uriParts = new UrlParts(testUri);

            Assert.AreEqual("https://www.example.com:88", uriParts.ProtocolAndHost);
            Assert.AreEqual("1992", uriParts.QueryParameters["year"]);
            Assert.AreEqual("blue", uriParts.QueryParameters["color"]);
            Assert.AreEqual("/equipment/cars", uriParts.Path);
        }

        [Test]
        public void Ctor_WhenUriAndDictionaryIsProvided_DictionaryParametersOverride()
        {
            var parameters = new Dictionary<string,string>()
            {
                { "wheels", "4"},
                {"year", "2001"} // overrides query string parameter
            };
            var testUri = new Uri("https://www.example.com:88/equipment/cars?year=1992&color=blue");

            var uriParts = new UrlParts(testUri, parameters);

            Assert.AreEqual("2001", uriParts.QueryParameters["year"]);
            Assert.AreEqual("blue", uriParts.QueryParameters["color"]);
            Assert.AreEqual("4", uriParts.QueryParameters["wheels"]);
        }

        [Test]
        public void BuildUri_AllPartsAreRestored()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "wheels", "4"},
                {"year", "2001"} // overrides query string parameter
            };
            var inputUri = new Uri("https://www.example.com:88/equipment/cars?year=1992&color=blue");

            var uriParts = new UrlParts(inputUri, parameters);

            Uri resultUri = uriParts.BuildUri();

            Assert.AreEqual(inputUri.Scheme, resultUri.Scheme);
            Assert.AreEqual(inputUri.Host, resultUri.Host);
            Assert.AreEqual(inputUri.Port, resultUri.Port);
            Assert.AreEqual(inputUri.AbsolutePath, resultUri.AbsolutePath);
            
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(resultUri.Query);

            Assert.AreEqual("4", queryParameters["wheels"]);
            Assert.AreEqual("2001", queryParameters["year"]);
            Assert.AreEqual("blue", queryParameters["color"]);

        }
    }
}
