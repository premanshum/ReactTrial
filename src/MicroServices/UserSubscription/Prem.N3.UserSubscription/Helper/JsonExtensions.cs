using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prem.N3.UserSubscription.Helper
{
    public static class JsonExtensions
    {
        public static string AsJson<TPoco>(this TPoco poco)
        {
            return JsonConvert.SerializeObject(poco);
        }
        public static string AsJson<TPoco>(this TPoco poco, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(poco, settings);
        }
        public static string AsJson<TPoco>(this TPoco poco, Formatting formatting)
        {
            return JsonConvert.SerializeObject(poco, formatting);
        }
        public static TPoco AsPoco<TPoco>(this string json, JsonSerializerSettings jsonSettings = null)
            where TPoco : class
        {
            return json.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<TPoco>(json, jsonSettings);
        }

    }
}
