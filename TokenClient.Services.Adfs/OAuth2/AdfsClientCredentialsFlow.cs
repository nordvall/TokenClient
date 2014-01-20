using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Adfs.OAuth2
{
    public class AdfsClientCredentialsFlow : ClientCredentialsFlowBase
    {
        public AdfsClientCredentialsFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }
        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, AdfsConstants.OAuthUrlPath); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters["scope"] = _parameters.Resource;

            return parameters;
        }
    }
}
