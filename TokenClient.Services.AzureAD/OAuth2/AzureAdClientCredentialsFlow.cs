using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.AzureAd.OAuth2
{
    public class AzureAdClientCredentialsFlow : ClientCredentialsFlowBase
    {
        public AzureAdClientCredentialsFlow(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest)
            : base(serviceUri, tokenRequest)
        {
            ValidateUri(serviceUri);
        }

        public AzureAdClientCredentialsFlow(Uri serviceUri, ClientCredentialsTokenRequest tokenRequest, IHttpClient httpAdapter)
            : base(serviceUri, tokenRequest, httpAdapter)
        {
            ValidateUri(serviceUri);
        }

        private static void ValidateUri(Uri serviceUri)
        {
            Guid validatedTenantId = Guid.Empty;

            if (Guid.TryParse(serviceUri.AbsolutePath, out validatedTenantId))
            {
                throw new ArgumentException("BaseUri should include tenant ID.");
            }
        }

        protected override Uri TokenEndpoint
        {
            get { return new Uri(_serviceUri, string.Format("{0}/{1}", _serviceUri.AbsolutePath, AzureAdConstants.OAuthTokenPath)); }
        }

        protected override Dictionary<string,string> CreateAccessTokenRequestParameters()
        {
            Dictionary<string,string> parameters = base.CreateAccessTokenRequestParameters();
            parameters.Remove("scope");
            parameters["resource"] = _tokenRequest.Scope;

            return parameters;
        }
    }
}
