using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public class AccessTokenErrorResponse
    {
        [JsonProperty("error")]
        public string ErrorCode { get; private set; }

        [JsonProperty("error_description")]
        public string Description { get; private set; }
    }
}
