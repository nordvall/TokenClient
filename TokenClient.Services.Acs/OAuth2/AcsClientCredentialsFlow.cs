using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Acs.OAuth2
{
    public class AcsClientCredentialsFlow : ClientCredentialsFlowBase
    {
        public AcsClientCredentialsFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }

        public AcsClientCredentialsFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters, IOAuthHttpAdapter httpAdapter)
            : base(serviceUri, credentials, parameters, httpAdapter)
        {

        }

        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuthUrlPath); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters["scope"] = _parameters.Resource;

            return parameters;
        }
    }
}
