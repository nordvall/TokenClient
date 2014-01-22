using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
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

        public AcsAuthorizationCodeFlow(Uri serviceUri, ClientCredentials credentials, RequestParameters parameters, IOAuthHttpAdapter httpAdapter)
            : base(serviceUri, credentials, parameters, httpAdapter)
        {

        }

        protected override Uri AuthorizationEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuthUrlPath); }
        }

        protected override Uri TokenRequestEndpoint
        {
            get { return new Uri(_serviceUri, AcsConstants.OAuthUrlPath); }
        }

        protected override Dictionary<string, string> GetAuthorizationRequestParameters()
        {
            throw new NotSupportedException("ACS needs delegations to be pre-created by the management API");
        }

        protected override Dictionary<string, string> CreateAccessTokenRequestParameters()
        {
            return base.CreateAccessTokenRequestParameters();
        }
    }
}
