using Newtonsoft.Json.Linq;

namespace WhatIfF1.Adapters
{
    public class APIResult
    {
        public JObject Data { get; }

        public bool Success { get; }

        public static APIResult Fail => new APIResult();

        public APIResult(JObject data)
        {
            Data = data;
            Success = true;
        }

        private APIResult()
        {
            Success = false;
        }
    }
}
