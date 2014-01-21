using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenClient.Protocols.OAuth2
{
    public static class DictionaryExtensions
    {
        public static FormUrlEncodedContent ToFormUrlEncodedContent(this Dictionary<string,string> dictionary)
        {
            return new FormUrlEncodedContent(dictionary);
        }

        public static string ToJsonString(this Dictionary<string, string> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }

        public static void AddRange(this Dictionary<string, string> dictionary, Dictionary<string, string> childDictionary)
        {
            foreach (string key in childDictionary.Keys)
            {
                dictionary[key] = childDictionary[key];
            }
        }
    }
}
