using Newtonsoft.Json.Linq;
using System;
using System.IO;
using WhatIfF1.Adapters;
using WhatIfF1.Logging;

namespace WhatIfF1.Modelling.Tracks
{
    public class Track
    {
        public string TrackName { get; }
        public double TrackLength { get; }
        public string CountryName { get; }
        public string LocationName { get; }
        public string WikipediaURL { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public string FlagFilePath { get; }

        public Track(JObject trackJson)
        {
            TrackName = trackJson["circuitName"].Value<string>();
            WikipediaURL = trackJson["url"].Value<string>();

            JObject locJson = trackJson["Location"].ToObject<JObject>();

            CountryName = locJson["country"].Value<string>();
            LocationName = locJson["locality"].Value<string>();
            Latitude = locJson["lat"].Value<double>();
            Longitude = locJson["long"].Value<double>();

            string flagsFolder = FileAdapter.Instance.CountryFlagsRoot;
            string flagFilePath = Path.Combine(flagsFolder, $"{CountryName}.png");

            if (!File.Exists(flagFilePath))
            {
                Logger.Instance.Warn($"Could not find a flag file at \"{flagFilePath}\". Using default flag");

                flagFilePath = Path.Combine(flagsFolder, "default.png");

                if (!File.Exists(flagFilePath))
                {
                    throw new TrackException($"Default flag file was not found at \"{flagFilePath}\"");
                }
            }

            FlagFilePath = flagFilePath;

            // TODO - this
            TrackLength = 5000;
        }
    }
}
