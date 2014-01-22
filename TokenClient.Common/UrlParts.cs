using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace TokenClient.Common
{
    public class UrlParts
    {
        public UrlParts(Uri uri)
        {
            QueryParameters = new Dictionary<string, string>();
            StoreUriValues(uri);
        }

        public UrlParts(Uri uri, Dictionary<string, string> queryParameters)
            : this(uri)
        {
            StoreQueryParameters(queryParameters);
        }

        public string ProtocolAndHost { get; set; }

        public string Path { get; set; }

        public Dictionary<string, string> QueryParameters { get; private set; }
        
        private void StoreUriValues(Uri uri)
        {
            if (uri.IsDefaultPort)
            {
                ProtocolAndHost = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            }
            else
            {
                ProtocolAndHost = string.Format("{0}://{1}:{2}", uri.Scheme, uri.Host, uri.Port);
            }

            Path = uri.AbsolutePath;

            NameValueCollection urlParameters = HttpUtility.ParseQueryString(uri.Query);

            foreach (string key in urlParameters.AllKeys)
            {
                QueryParameters.Add(key, urlParameters[key]);
            }
        }

        private void StoreQueryParameters(Dictionary<string, string> queryParameters)
        {
            foreach (var item in queryParameters)
            {
                // Overwrite existing keys
                QueryParameters[item.Key] = item.Value;
            }
        }
        
        public Uri BuildUri()
        {
            var builder = new UriBuilder(ProtocolAndHost);
            builder.Path = Path;
            builder.Query = CreateQueryStringFromParameters(QueryParameters);

            return builder.Uri;
        }

        private string CreateQueryStringFromParameters(Dictionary<string, string> parameters)
        {
            Func<string, string> combineKeyAndValue = (key) =>
            {
                string encodedKey = HttpUtility.UrlEncode(key);
                string encodedValue = HttpUtility.UrlEncode(parameters[key]);
                return string.Format("{0}={1}", encodedKey, encodedValue);
            };

            IEnumerable<string> keyValuePairs = parameters.Keys.Select(combineKeyAndValue);

            return string.Join("&", keyValuePairs);
        }
    }
}
