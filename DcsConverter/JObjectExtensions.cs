using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcsConverter
{
    public static class JObjectExtensions
    {
        public static IDictionary<string, object> ToDictionary(this JObject @object)
        {
            var result = @object.ToObject<Dictionary<string, object>>();

            var jObjectKeys = (from r in result
                               let key = r.Key
                               let value = r.Value
                               where value.GetType() == typeof(JObject)
                               select key).ToList();

            var jArrayKeys = (from r in result
                              let key = r.Key
                              let value = r.Value
                              where value.GetType() == typeof(JArray)
                              select key).ToList();
            /*
            foreach (KeyValuePair<string, object> entry in result)
            {
                if(entry.Value.GetType() == typeof(JProperty))
                {
                    result.Remove(entry.Key);
                }
            }
            */
            jArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
            jObjectKeys.ForEach(key => result[key] = ToDictionary(result[key] as JObject));

            return result;
        }

    }
}
