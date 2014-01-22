using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenClient.Common;
using TokenClient.Protocols.OAuthWrap.Http;

namespace TokenClient.Protocols.OAuthWrap
{
    public class ClientAccountPasswordFlow
    {
        protected readonly ClientCredentials _clientCredentials;
        protected readonly RequestParameters _parameters;
        protected readonly Uri _serviceUri;
        protected readonly IHttpClient _httpClient;

        public ClientAccountPasswordFlow(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters)
            : this(serviceUri, clientCredentials, parameters, new WrapHttpClient())
        {

        }

        public ClientAccountPasswordFlow(Uri serviceUri, ClientCredentials clientCredentials, RequestParameters parameters, IHttpClient httpClient)
        {
            _serviceUri = serviceUri;
            _clientCredentials = clientCredentials;
            _parameters = parameters;
            _httpClient = httpClient;
        }
    }
}
