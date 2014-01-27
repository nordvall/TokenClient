using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Common.Tokens
{
    public class WebToken
    {
        public WebToken(string token)
        {
            Token = token;
            Expiration = DateTime.Now;
        }

        public DateTime Expiration { get; private set; }

        public string Token { get; private set; }
    }
}
