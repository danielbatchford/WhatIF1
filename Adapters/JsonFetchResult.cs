using Newtonsoft.Json.Linq;
using WhatIfF1.Adapters.Interfaces;

namespace WhatIfF1.Adapters
{
    public class JsonFetchResult : IFetchResult<JToken>
    {
        public JToken Data { get; }

        public bool Success { get; }

        public static JsonFetchResult Fail => new JsonFetchResult();

        public JsonFetchResult(JToken data)
        {
            Data = data;
            Success = true;
        }

        private JsonFetchResult()
        {
            Success = false;
        }
    }
}
