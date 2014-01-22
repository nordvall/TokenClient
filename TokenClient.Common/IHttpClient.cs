using System;
using TokenClient.Common;
namespace TokenClient.Common
{
    public interface IHttpClient
    {
        ProtocolResponse SendRequest(ProtocolRequest oauthRequest);
    }
}
