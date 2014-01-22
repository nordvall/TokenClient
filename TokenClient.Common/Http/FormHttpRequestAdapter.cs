using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common.Http
{
    public class FormHttpRequestAdapter : HttpRequestAdapter
    {
        protected override void AddHttpRequestContent(ProtocolRequest protocolRequest, HttpRequestMessage httpRequest)
        {
            httpRequest.Content = new FormUrlEncodedContent(protocolRequest.BodyParameters);
        }
    }
}
