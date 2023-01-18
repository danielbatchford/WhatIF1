using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public static class TelemetryParser
    {
        public static IDictionary<Driver, IList<VelocityDistanceTimeContainer>> ParseTelemetryJson(IEnumerable<Driver> drivers, IDictionary<Driver, ICollection<int>> lapTimes, JObject telemetryJson)
        {
            JArray channelValues = (JArray)telemetryJson["ChannelValues"];

            // Find first time stamp and parse it to a DateTime
            string startTimeString = channelValues[0]["Utc"].ToString(Formatting.None);

            // Remove start and end quotes from this string
            startTimeString = startTimeString.Substring(1, startTimeString.Length - 2);

            DateTime startTime = DateTimeExtensions.FromLongIso8601String(startTimeString);

            var rawTimeStampsDict = drivers.ToDictionary(driver => driver, _ => new List<TelemetryTimeStamp>(channelValues.Count));

            var sortedDrivers = telemetryJson["ChannelKeys"].Select(num => drivers.Single(driver => driver.DriverNumber == num.ToObject<int>())).ToList();

            foreach (JToken entry in channelValues)
            {
                string currentTimeString = entry["Utc"].ToString(Formatting.None);

                // Remove start and end quotes from this string
                currentTimeString = currentTimeString.Substring(1, currentTimeString.Length - 2);

                DateTime currentTime = DateTimeExtensions.FromLongIso8601String(currentTimeString);
                int ms = (int)currentTime.Subtract(startTime).TotalMilliseconds;

                for (int driverIdx = 0; driverIdx < sortedDrivers.Count; driverIdx++)
                {
                    int velocity = (int)entry["Velocities"][driverIdx];
                    rawTimeStampsDict[sortedDrivers[driverIdx]].Add(new TelemetryTimeStamp { Ms = ms, Velocity = velocity });
                }
            }

            // TODO - remove
            /*
            foreach(Driver driver in drivers)
            {
                string path = Path.Combine(@"C:\Users\Daniel Batchford\Desktop\temp", $"{driver.DriverLetters}.csv");

                StringBuilder stringBuilder = new StringBuilder();

                foreach(var ts in rawTimeStampsDict[driver])
                {
                    stringBuilder.AppendLine($"{ts.Ms}, {ts.Velocity}");
                }

                File.WriteAllText(path, stringBuilder.ToString());
            }
            */

            return drivers.ToDictionary(driver => driver, driver => BuildVDTContainers(rawTimeStampsDict[driver], lapTimes[driver].ToList()));
        }

        public static IList<VelocityDistanceTimeContainer> BuildVDTContainers(IList<TelemetryTimeStamp> timeStamps, IList<int> lapTimes) 
        {
            int currentLapIdx = 0;
            int cumulativeLapSum = lapTimes[0];

            var splitIndexes = new List<int>(lapTimes.Count - 1);

            for (int i = 0; i < timeStamps.Count; i++)
            {
                if (timeStamps[i].Ms > cumulativeLapSum)
                {
                    splitIndexes.Add(i);
                    currentLapIdx++;
                    cumulativeLapSum += lapTimes[currentLapIdx];
                }
            }

            IList<List<TelemetryTimeStamp>> splitTimeStamps = timeStamps.SplitByIndexList(splitIndexes);
            return splitTimeStamps.Select(timeStamp => new VelocityDistanceTimeContainer(timeStamp)).ToList();
        }
    }
}
