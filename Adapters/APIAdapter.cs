using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WhatIfF1.Logging;

namespace WhatIfF1.Adapters
{
    public static class APIAdapter
    {
        private static readonly string _f1APIRoot = (string)Properties.Settings.Default["f1APIRoot"];

        public static async Task<APIResult> Get(string baseAddress, string relativeAddress)
        {
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress)})
            {
                // Only allow json responses
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(relativeAddress);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Error($"Received invalid success code: {response.ReasonPhrase}");
                    return APIResult.Fail;
                }
            }

            string responseString = await response.Content.ReadAsStringAsync();

            try
            {
                JObject responseJson = JObject.Parse(responseString);

                return new APIResult(responseJson, true);
            }
            catch (JsonReaderException e)
            {
                Logger.Instance.Error($"Returned response was not valid JSON: {e.Message}");
                return APIResult.Fail;
            }
        }

        public static async Task<APIResult> GetFromF1API(string relativeAddress)
        {
            // Add on ".json" to the request address if it is not present, to fetch json data only
            if (!relativeAddress.EndsWith(".json"))
            {
                relativeAddress += ".json";
            }

            // see https://documenter.getpostman.com/view/11586746/SztEa7bL#intro
            return await Get(_f1APIRoot, relativeAddress);
        }
    }
}
