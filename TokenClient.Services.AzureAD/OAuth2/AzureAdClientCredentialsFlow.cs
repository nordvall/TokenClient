using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.AzureAd.OAuth2
{
    public class AzureAdClientCredentialsFlow : ClientCredentialsFlowBase
    {
        public AzureAdClientCredentialsFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {
            Guid validatedTenantId = Guid.Empty;

            if (Guid.TryParse(serviceUri.AbsolutePath, out validatedTenantId))
            {
                throw new ArgumentException("BaseUri should include tenant ID.");
            }
        }

        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, _serviceUri.AbsolutePath + AzureAdConstants.OAuthUrlPath); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters.Remove("scope");
            parameters["resource"] = _parameters.Resource;

            return parameters;
        }
    }
}
