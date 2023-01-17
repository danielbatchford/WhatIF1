using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Logging;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Adapters
{
    public static class APIAdapter
    {
        private static readonly string _ergastAPIRoot = (string)Properties.Settings.Default["ergastAPIRoot"];
        private static readonly string _liveTimingAPIRoot = (string)Properties.Settings.Default["liveTimingAPIRoot"];

        private static async Task<APIResult> GetJson(string baseAddress, string relativeAddress)
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
                    return APIResult.Fail;
                }
            }

            string responseString = await response.Content.ReadAsStringAsync();

            // Sometimes, the response string is wrapped in b''
            // Remove this if this is the case
            if(responseString.StartsWith("b\'") && responseString.EndsWith("'"))
            {
                responseString = responseString.TrimStart(new char[] { 'b', '\'' }).TrimEnd('\'');
            }

            if (TryParseResponseJson(responseString, out JObject json))
            {
                return new APIResult(json);
            }
            else
            {
                Logger.Instance.Error("Failed to parse the response json");
                return APIResult.Fail;
            }
        }

        public static async Task<APIResult> GetFromErgastAPI(string relativeAddress)
        {
            return await GetJson(_ergastAPIRoot, relativeAddress);
        }

        public static async Task<APIResult> GetVelocityJsonFromLiveTimingAPI(DateTime eventDate, string eventName)
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
                    return APIResult.Fail;
                }
            }

            string responseString = await response.Content.ReadAsStringAsync();

            try
            {
                // Response data needs to be decoded here
                JObject json = DecodeLiveTimingAPIResultString(responseString);
                return new APIResult(json);
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to parse live timing response text: {e.Message}");
                return APIResult.Fail;
            }

        }
        private static bool TryParseResponseJson(string responseString, out JObject json)
        {
            try
            {
                json = JObject.Parse(responseString);
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
        private static JObject DecodeLiveTimingAPIResultString(string raw)
        {
            // TODO - remove 
            // File.WriteAllText(@"C:\Users\Daniel Batchford\Desktop\test.txt", raw);

            var lines = raw.Split('\n');

            const int timestampLength = 13;

            StringBuilder resultBuilder = new StringBuilder();

            // Ignore last line of text, as it is simply an empty line
            foreach(string line in lines.Take(lines.Length - 1))
            {
                // Indexes account for leading " and tailing \r"
                string dataString = line.Substring(timestampLength, line.Length - timestampLength - 2);

                byte[] toDecode = Convert.FromBase64String(dataString);

                using (Stream compressedStream = new MemoryStream(toDecode))
                {
                    using(Stream uncompressedStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    {
                        StreamReader reader = new StreamReader(uncompressedStream, Encoding.UTF8);
                        resultBuilder.Append(reader.ReadToEnd());
                    }
                }
            }

            // TODO - remove 
            // File.WriteAllText(@"C:\Users\Daniel Batchford\Desktop\testdecoded.json", resultBuilder.ToString());

            JObject primitive = JObject.Parse(resultBuilder.ToString());

            return new JObject();
        }

    }
}
