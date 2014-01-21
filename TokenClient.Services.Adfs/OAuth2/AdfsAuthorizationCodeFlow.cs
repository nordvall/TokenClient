using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Adfs.OAuth2
{
    public class AdfsAuthorizationCodeFlow : AuthorizationCodeFlowBase
    {
        public AdfsAuthorizationCodeFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }

        protected override Uri TokenRequestEndpoint
        {
            get { return new Uri(_serviceUri, AdfsConstants.OAuthUrlPath); }
        }

        protected override Uri AuthorizationEndpoint
        {
            get { return new Uri(_serviceUri, AdfsConstants.OAuthUrlPath); }
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
