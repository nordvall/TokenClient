using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common
{
    public class ProtocolRequest
    {
        public ProtocolRequest()
        {
            BodyParameters = new Dictionary<string, string>();
        }

        public UrlParts Url { get; set; }

        public Dictionary<string,string> BodyParameters { get; set; }
    }
}
