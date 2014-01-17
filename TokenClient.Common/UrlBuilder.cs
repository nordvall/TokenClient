using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TokenClient
{
    public class UrlBuilder
    {
        private UriBuilder _innerBuilder;

        public UrlBuilder(Uri baseUri)
        {
            _innerBuilder = new UriBuilder(baseUri);

            Parameters = HttpUtility.ParseQueryString(_innerBuilder.Query);
        }

        public NameValueCollection Parameters { get; private set; }

        public Uri Build()
        { 
            BuildQueryString();

            return _innerBuilder.Uri;
        }

        private void BuildQueryString()
        {
            var builder = new StringBuilder();
            
            for (int i = 0; i < Parameters.Count; i++)
            {
                string encodedKey = HttpUtility.UrlEncode(Parameters.Keys[i]);
                string encodedValue = HttpUtility.UrlEncode(Parameters[i]);

                builder.AppendFormat("{0}={1}&", encodedKey, encodedValue);
            }

            _innerBuilder.Query = builder.ToString();
        }
    }
}
