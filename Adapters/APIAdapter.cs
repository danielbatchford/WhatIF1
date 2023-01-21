using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Logging;

namespace WhatIfF1.Adapters
{
    public static class APIAdapter
    {
        private static readonly string _ergastAPIRoot = (string)Properties.Settings.Default["ergastAPIRoot"];
        private static readonly string _liveTimingAPIRoot = (string)Properties.Settings.Default["liveTimingAPIRoot"];

        private static async Task<FetchResult> GetJson(string baseAddress, string relativeAddress)
        {
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) })
            {
                // Only allow json responses
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(relativeAddress);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Error($"Received invalid success code: {response.ReasonPhrase}");
                    return FetchResult.Fail;
                }
            }

            string responseString = await response.Content.ReadAsStringAsync();

            // Sometimes, the response string is wrapped in b''
            // Remove this if this is the case
            if (responseString.StartsWith("b\'") && responseString.EndsWith("'"))
            {
                responseString = responseString.TrimStart(new char[] { 'b', '\'' }).TrimEnd('\'');
            }

            if (TryParseResponseJson(responseString, out JToken json))
            {
                return new FetchResult(json);
            }
            else
            {
                Logger.Instance.Error("Failed to parse the response json");
                return FetchResult.Fail;
            }
        }

        public static async Task<FetchResult> GetFromErgastAPI(string relativeAddress)
        {
            return await GetJson(_ergastAPIRoot, relativeAddress);
        }

        public static async Task<FetchResult> GetTelemetryJsonFromLiveTimingAPI(string eventName, DateTime eventDate)
        {
            string dateString = eventDate.ToString("yyyy-MM-dd");

            string relativeAddress = $"{eventDate.Year}/{dateString} {eventName}/{dateString} Race/CarData.z.jsonStream";

            relativeAddress = relativeAddress.Replace(' ', '_');

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_liveTimingAPIRoot) })
            {
                response = await client.GetAsync(relativeAddress);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Error($"Received invalid success code: {response.ReasonPhrase}");
                    return FetchResult.Fail;
                }
            }

            string responseString = await response.Content.ReadAsStringAsync();

            try
            {
                // Response data needs to be decoded here
                JToken json = DecodeLiveTimingAPIResultToJson(responseString);
                return new FetchResult(json);
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to parse live timing response text: {e.Message}");
                return FetchResult.Fail;
            }
        }
        private static bool TryParseResponseJson(string responseString, out JToken json)
        {
            try
            {
                json = JToken.Parse(responseString);
                return true;

            }
            catch (JsonReaderException e)
            {
                Logger.Instance.Error($"Returned response was not valid JSON: {e.Message}");
                json = null;
                return false;
            }
        }

        /// <summary>
        /// Decodes the live timing api result string into a JObject
        /// See https://github.com/theOehrly/Fast-F1, api.py for parsing logic
        /// </summary>
        /// <returns>JObject of decoded data</returns>
        private static JToken DecodeLiveTimingAPIResultToJson(string raw)
        {
            var lines = raw.Split('\n');

            const int timestampLength = 13;
            const string channelVelocityLabel = "2";
            const int minNonZerosForRaceStart = 5;
            JArray outputArray = new JArray();

            // Used to infer when the race has started
            bool raceStarted = false;

            string[] channelKeys = null;
            int[] velocities = null;

            // Ignore last line of text, as it is simply an empty line
            foreach (string line in lines.Take(lines.Length - 1))
            {
                // Indexes account for leading " and tailing \r"
                string dataString = line.Substring(timestampLength, line.Length - timestampLength - 2);

                byte[] toDecode = Convert.FromBase64String(dataString);

                using (Stream compressedStream = new MemoryStream(toDecode))
                {
                    using (Stream uncompressedStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    {
                        StreamReader streamReader = new StreamReader(uncompressedStream, Encoding.UTF8);

                        JToken jsonObj = JToken.Parse(streamReader.ReadToEnd());

                        JArray entriesArray = (JArray)jsonObj["Entries"];

                        // Extract driver channel keys here for optimisation if they have not been assigned
                        if (channelKeys == null)
                        {
                            channelKeys = ((JObject)entriesArray[0]["Cars"]).Properties().Select(prop => prop.Name).ToArray();
                            velocities = new int[channelKeys.Length];
                        }

                        foreach (JToken fullEntry in entriesArray)
                        {
                            int numNonZerosFound = 0;

                            for (int i = 0; i < channelKeys.Length; i++)
                            {
                                velocities[i] = (int)fullEntry["Cars"][channelKeys[i]]["Channels"][channelVelocityLabel];

                                if (velocities[i] > 0)
                                {
                                    numNonZerosFound++;
                                }
                            }

                            if (!raceStarted && numNonZerosFound >= minNonZerosForRaceStart)
                            {
                                raceStarted = true;
                            }

                            if (raceStarted)
                            {
                                outputArray.Add(new JObject
                                {
                                    ["Utc"] = fullEntry["Utc"],
                                    ["Velocities"] = new JArray(velocities)
                                });
                            }
                        }
                    }
                }
            }

            return new JObject
            {
                ["ChannelKeys"] = new JArray(channelKeys),
                ["ChannelValues"] = outputArray
            };
        }

    }
}
