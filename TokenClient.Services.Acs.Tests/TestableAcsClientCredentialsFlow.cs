using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;
using TokenClient.Services.Acs.OAuth2;

namespace TokenClient.Services.Acs.Tests
{
    class TestableAcsClientCredentialsFlow : AcsClientCredentialsFlow
    {
        public TestableAcsClientCredentialsFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }

        protected override HttpClient HttpClient
        {
            get
            {
                return new HttpClient(HttpHandler);
            }
        }

        public FakeHttpHandler HttpHandler { get; set; }
    }
}
