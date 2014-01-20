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
        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, "v2/oauth2-13/"); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters["scope"] = _parameters.Resource;

            return parameters;
        }
    }
}
