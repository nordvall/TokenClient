using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common
{
    public class ProtocolResponse
    {
        public ProtocolResponse()
            : this(new Dictionary<string, string>())
        {
            
        }

        public ProtocolResponse(Dictionary<string, string> bodyParameters)
        {
            BodyParameters = bodyParameters;
        }

        public Dictionary<string,string> BodyParameters { get; private set; }
    }
}
