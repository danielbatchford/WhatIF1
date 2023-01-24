using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.Drivers.Telemetry.Interfaces;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public class TelemetryParser : ITelemetryParser<JObject>
    {
        private readonly double _trackLength;

        public TelemetryParser(double trackLength)
        {
            _trackLength = trackLength;
        }

        public IDictionary<IDriver, IList<IVelocityDistanceTimeContainer>> ParseTelemetryJson(IEnumerable<IDriver> drivers, IDictionary<IDriver, ICollection<int>> lapTimes, JObject telemetryJson)
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

            return BuildVDTContainersDict(rawTimeStampsDict, lapTimes);
        }

        private IDictionary<IDriver, IList<IVelocityDistanceTimeContainer>> BuildVDTContainersDict(IDictionary<IDriver, List<TelemetryTimeStamp>> allTimeStampsDict, IDictionary<IDriver, ICollection<int>> lapTimesDict)
        {
            allTimeStampsDict = CutoffPreRaceTimeStamps(allTimeStampsDict);

            var vdtContainersDict = new Dictionary<IDriver, IList<IVelocityDistanceTimeContainer>>(allTimeStampsDict.Count);

            foreach (Driver driver in allTimeStampsDict.Keys)
            {
                vdtContainersDict.Add(driver, BuildVDTContainersList(allTimeStampsDict[driver], lapTimesDict[driver].ToList()));
            }

            return vdtContainersDict;
        }

        private List<IVelocityDistanceTimeContainer> BuildVDTContainersList(List<TelemetryTimeStamp> timeStamps, List<int> lapTimes)
        {
            var vdtContainers = new List<IVelocityDistanceTimeContainer>(lapTimes.Count);
            int currentLapIdx = 0;
            int cumulativeLapTime = lapTimes[0];
            int msLapOffset = 0;

            var timeStampsForLap = new List<TelemetryTimeStamp>();
            bool nonZeroVelocityFound = false;

            foreach (var timeStamp in timeStamps)
            {
                if (timeStamp.Ms < cumulativeLapTime)
                {
                    timeStampsForLap.Add(new TelemetryTimeStamp { Ms = timeStamp.Ms - msLapOffset, Velocity = timeStamp.Velocity });

                    if (timeStamp.Velocity > 0)
                    {
                        nonZeroVelocityFound = true;
                    }
                }
                else
                {
                    // Move on to next lap

                    // Add in time stamps for very start and end of lap
                    timeStampsForLap.Insert(0, new TelemetryTimeStamp { Ms = 0, Velocity = timeStampsForLap[0].Velocity });
                    timeStampsForLap.Add(new TelemetryTimeStamp { Ms = lapTimes[currentLapIdx], Velocity = timeStampsForLap.Last().Velocity });

                    vdtContainers.Add(new VelocityDistanceTimeContainer(currentLapIdx + 1, _trackLength, timeStampsForLap));
                    msLapOffset += timeStampsForLap[timeStampsForLap.Count - 1].Ms;
                    timeStampsForLap.Clear();
                    currentLapIdx++;

                    // If all timestamps have v = 0, the car has retired, or if end of race is reached, return
                    if (currentLapIdx == lapTimes.Count || !nonZeroVelocityFound)
                    {
                        return vdtContainers;
                    }
                    else
                    {
                        cumulativeLapTime += lapTimes[currentLapIdx];
                        nonZeroVelocityFound = false;
                    }
                }
            }

            throw new ArgumentException("Provided timestamps range did not fill full range of lap times");
        }

        /// <summary>
        /// Cut off all samples from lap to grid / formation lap - first sample should be the start of the race
        /// </summary>
        /// <param name="allTimeStampsDict"></param>
        private IDictionary<IDriver, List<TelemetryTimeStamp>> CutoffPreRaceTimeStamps(IDictionary<IDriver, List<TelemetryTimeStamp>> allTimeStampsDict)
        {
            int nSamples = allTimeStampsDict.First().Value.Count;

            var drivers = allTimeStampsDict.Keys.ToList();
            var allTimeStamps = allTimeStampsDict.Values.ToList();

            var allCarsStationary = Enumerable.Repeat(true, nSamples).ToList();

            foreach (var timestamps in allTimeStamps)
            {
                for (int i = 0; i < nSamples; i++)
                {
                    allCarsStationary[i] = allCarsStationary[i] && timestamps[i].Velocity == 0;
                }
            }

            // The race start index here is defined as the index of the last true value in the allCarsStationary list
            int lastStationaryIdx = allCarsStationary.LastIndexOf(true);

            int msOffset = allTimeStamps[0][lastStationaryIdx].Ms;

            var outputTimeStamps = new Dictionary<IDriver, List<TelemetryTimeStamp>>(drivers.Count);

            for (int driverIdx = 0; driverIdx < drivers.Count; driverIdx++)
            {
                var timeStamps = allTimeStamps[driverIdx];

                var offsetCutoffTimeStamps = timeStamps.GetRange(lastStationaryIdx, timeStamps.Count - lastStationaryIdx);
                offsetCutoffTimeStamps = offsetCutoffTimeStamps.ConvertAll(ts => new TelemetryTimeStamp { Ms = ts.Ms - msOffset, Velocity = ts.Velocity });
                outputTimeStamps.Add(drivers[driverIdx], offsetCutoffTimeStamps);
            }

            return outputTimeStamps;
        }
    }
}
