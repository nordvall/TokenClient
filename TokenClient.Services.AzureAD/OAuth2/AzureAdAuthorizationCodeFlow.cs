using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.AzureAd.OAuth2
{
    public class AzureAdAuthorizationCodeFlow : AuthorizationCodeFlowBase
    {
        public AzureAdAuthorizationCodeFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {
            Guid validatedTenantId = Guid.Empty;

            if (Guid.TryParse(serviceUri.AbsolutePath, out validatedTenantId))
            {
                throw new ArgumentException("BaseUri should include tenant ID.");
            }
        }

        protected override Uri TokenRequestEndpoint
        {
            get { return new Uri(_serviceUri, AzureAdConstants.OAuthUrlPath); }
        }

        protected override Dictionary<string, string> GetAuthorizationRequestParameters()
        {
            Dictionary<string, string> parameters = base.GetAuthorizationRequestParameters();
            parameters.Remove("scope");
            parameters.Add("resource", _parameters.Resource);

            return parameters;
        }

        protected override Dictionary<string, string> CreateAccessTokenRequestParameters()
        {
            return base.CreateAccessTokenRequestParameters();
        }
    }
}
