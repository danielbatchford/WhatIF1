using Newtonsoft.Json.Linq;

namespace WhatIfF1.Adapters
{
    public class APIResult
    {
        public JObject Data { get; }

        public bool Success { get; }

        public static APIResult Fail => new APIResult(null, false);

        public APIResult(JObject data, bool success)
        {
            Data = data;
            Success = success;
        }
    }
}
