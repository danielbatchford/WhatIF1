using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task<APIResult> GetLapsFromF1API(int year, int round, int numLaps)
        {
            // A call must be made for each lap of the race, as the api does not support multiple lap requests at once

            // Build list of urls
            var urls = new List<string>(numLaps);

            for (int lapIndex = 0; lapIndex < numLaps; lapIndex++)
            {
                urls.Add($"{year}/{round}/laps/{lapIndex + 1}");
            }

            // Create a new list of tasks from the list of urls
            var tasks = urls.ConvertAll(url => GetFromF1API(url));

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            if(!tasks.All(task => task.Result.Success))
            {
                Logger.Instance.Error("One or more lap time API calls failed");
                return APIResult.Fail;
            }

            // Build combined Json array from fetched laps
            JArray lapArray = new JArray();

            for (int lapIndex = 0; lapIndex < numLaps; lapIndex++)
            {
                // Get lap data from this lap's task
                JObject lapData = tasks[lapIndex].Result.Data;

                // Filter through the json to find the timing data for this lap
                JObject innerLapData = lapData["MRData"]["RaceTable"]["Races"][0]["Laps"][0].ToObject<JObject>();

                // Add a new object with the lap number and the lap's data
                lapArray.Add(new JObject
                {
                    { "lap", (lapIndex + 1).ToString() },
                    {"times", innerLapData }
                });
            }

            JObject lapObject = new JObject
            {
                {"laps", lapArray }
            };

            return new APIResult(lapObject, true);
        }
    }
}
