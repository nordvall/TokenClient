using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Protocols.OAuth2;

namespace TokenClient.Services.Acs.OAuth2
{
    public class AcsAuthorizationCodeFlow : AuthorizationCodeFlowBase
    {
        public AcsAuthorizationCodeFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters)
            : base(serviceUri, credentials, parameters)
        {

        }

        protected override Uri TokenRequestEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuthUrlPath); }
        }

        protected override NameValueCollection GetAuthorizationRequestParameters()
        {
            NameValueCollection parameters = base.GetAuthorizationRequestParameters();
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
