using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common.Http
{
    public abstract class HttpResponseAdapter
    {
        public ProtocolResponse CreateProtocolResponse(HttpResponseMessage httpResponse)
        {
            ThrowIfErrorResponse(httpResponse);
            var protocolResponse = new ProtocolResponse();
            AddBodyParameters(httpResponse, protocolResponse);
            
            return protocolResponse;
        }

        protected virtual void ThrowIfErrorResponse(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                throw new Exception(responseContent);
            }
        }

        protected abstract void AddBodyParameters(HttpResponseMessage httpResponse, ProtocolResponse protocolResponse);
    }
}
