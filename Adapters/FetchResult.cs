using Newtonsoft.Json.Linq;

namespace WhatIfF1.Adapters
{
    public class FetchResult
    {
        public JToken Data { get; }

        public bool Success { get; }

        public static FetchResult Fail => new FetchResult();

        public FetchResult(JToken data)
        {
            Data = data;
            Success = true;
        }

        private FetchResult()
        {
            Success = false;
        }
    }
}
