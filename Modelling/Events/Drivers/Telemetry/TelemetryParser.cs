using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public static class TelemetryParser
    {
        public static IDictionary<Driver, IList<VelocityDistanceTimeContainer>> ParseTelemetryJson(IEnumerable<Driver> drivers, IDictionary<Driver, ICollection<int>> lapTimes, JArray telemetryJson)
        {
            // Find first time stamp and parse it to a DateTime
            string startTimeString = telemetryJson[0]["Utc"].ToString(Formatting.None);

            // Remove start and end quotes from this string
            startTimeString = startTimeString.Substring(1, startTimeString.Length - 2);

            DateTime startTime = DateTimeExtensions.FromLongIso8601String(startTimeString);

            var rawTimeStampsDict = drivers.ToDictionary(driver => driver, _ => new List<TelemetryTimeStamp>(telemetryJson.Count));

            // Extract dict of driver numbers
            var driverNumbersDict = drivers.ToDictionary(driver => driver, driver => driver.DriverNumber.ToString());

            foreach (JToken entry in telemetryJson)
            {
                string currentTimeString = entry["Utc"].ToString(Formatting.None);

                // Remove start and end quotes from this string
                currentTimeString = currentTimeString.Substring(1, currentTimeString.Length - 2);

                DateTime currentTime = DateTimeExtensions.FromLongIso8601String(currentTimeString);
                int ms = (int)currentTime.Subtract(startTime).TotalMilliseconds;

                foreach (Driver driver in drivers)
                {
                    string driverNumberKey = driverNumbersDict[driver];

                    // "2" key denotes the velocity entry
                    double velocity = entry["Cars"][driverNumberKey]["Channels"]["2"].ToObject<double>();
                    rawTimeStampsDict[driver].Add(new TelemetryTimeStamp { Ms = ms, Velocity = velocity});
                }
            }

            return drivers.ToDictionary(driver => driver, driver => BuildVDTContainers(rawTimeStampsDict[driver], lapTimes[driver].ToList()));
        }

        public static IList<VelocityDistanceTimeContainer> BuildVDTContainers(IList<TelemetryTimeStamp> timeStamps, IList<int> lapTimes) 
        {
            int currentLapIdx = 0;

            var splitIndexes = new List<int>(lapTimes.Count - 1);

            for (int i = 0; i < timeStamps.Count; i++)
            {
                if (timeStamps[i].Ms > lapTimes[currentLapIdx])
                {
                    splitIndexes.Add(i);
                    currentLapIdx++;
                }
            }

            IList<IList<TelemetryTimeStamp>> splitTimeStamps = timeStamps.SplitByIndexes(splitIndexes);
            return splitTimeStamps.Select(timeStamp => new VelocityDistanceTimeContainer(timeStamp));
        }
    }
}
