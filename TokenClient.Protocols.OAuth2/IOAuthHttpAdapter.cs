using System;
namespace TokenClient.Protocols.OAuth2
{
    public interface IOAuthHttpAdapter
    {
        ProtocolResponse SendRequest(ProtocolRequest oauthRequest);
    }
}
