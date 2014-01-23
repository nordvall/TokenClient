using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Acs.OAuth2
{
    public class AcsClientCredentialsFlow : ClientCredentialsFlowBase
    {
        public AcsClientCredentialsFlow(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest)
            : base(serviceUri, tokenRequest)
        {

        }

        public AcsClientCredentialsFlow(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest, IHttpClient httpAdapter)
            : base(serviceUri, tokenRequest, httpAdapter)
        {

        }

        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuth2UrlPath); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters["scope"] = _tokenRequest.Scope;

            return parameters;
        }
    }
}
