using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.Drivers.Telemetry;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.Modelling.Tires;
using WhatIfF1.UI.Controller;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel : NotifyPropertyChangedWrapper, IEventModel
    {
        public static EventModel GetModel(string name, double trackLength, JArray driversJson, JArray lapTimesJson, JObject telemetryJson)
        {
            var drivers = Driver.GetDriversAndRetirementsListFromJSON(driversJson, out IDictionary<IDriver, bool> isDriverRetiredDict);
            int noOfDrivers = drivers.Count();

            var lapTimes = new Dictionary<IDriver, ICollection<int>>();

            // Initialise driver times dictionary
            foreach (IDriver driver in drivers)
            {
                lapTimes.Add(driver, new List<int>());
            }

            const string timeFormat = @"m\:ss\.fff";

            foreach (JObject lapTimeJson in lapTimesJson)
            {
                foreach (Driver driver in drivers)
                {
                    // If no element is found, this implies the driver has retired from the race
                    JToken timingObject = lapTimeJson["Timings"].SingleOrDefault(timing => timing["driverId"].ToObject<string>().Equals(driver.DriverID));

                    if (timingObject == default)
                    {
                        continue;
                    }

                    string timingString = timingObject["time"].ToObject<string>();

                    // Convert this timing string from format minute:second.millisecond to milliseconds
                    TimeSpan timeSpan = TimeSpan.ParseExact(timingString, timeFormat, CultureInfo.InvariantCulture);

                    lapTimes[driver].Add((int)timeSpan.TotalMilliseconds);
                }
            }

            var telemetryParser = new TelemetryParser(trackLength);

            var vdtContainers = telemetryParser.ParseTelemetryJson(drivers, lapTimes, telemetryJson);

            // Initialise number of laps as the max of the laps completed by each driver
            int noOfLaps = lapTimes.Values.Max(lt => lt.Count);

            // Initialise driver models

            var driverModels = new Dictionary<IDriver, IDriverModel>(noOfDrivers);

            foreach (IDriver driver in drivers)
            {
                driverModels.Add(driver, new DriverModel(lapTimes[driver], vdtContainers[driver], trackLength, noOfLaps, isDriverRetiredDict[driver]));
            }

            // Initialse the total time as the maximum time of the total times of each driver
            int totalTime = driverModels.Values.Max(dMOdel => dMOdel.DriverTotalTime);

            return new EventModel(name, noOfDrivers, noOfLaps, totalTime, driverModels);
        }

        private readonly IDictionary<IDriver, IDriverModel> _driverModels;

        public int NoOfDrivers { get; }

        public string Name { get; }

        private int _noOfLaps;
        public int NoOfLaps
        {
            get => _noOfLaps;
            set
            {
                if (_noOfLaps == value)
                {
                    return;
                }

                _noOfLaps = value;
                OnPropertyChanged();
            }
        }

        private int _totalTime;
        public int TotalTime
        {
            get => _totalTime;
            set
            {
                if (value == _totalTime)
                {
                    return;
                }

                _totalTime = value;
                OnPropertyChanged();
            }
        }

        private EventModel(string name, int noOfDrivers, int noOfLaps, int totalTime, IDictionary<IDriver, IDriverModel> driverModels)
        {
            Name = name;
            NoOfDrivers = noOfDrivers;

            _noOfLaps = noOfLaps;
            _totalTime = totalTime;
            _driverModels = driverModels;
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<IDriver> GetDrivers()
        {
            return new List<IDriver>(_driverModels.Keys);
        }

        public IEnumerable<IDriverStanding> GetStandingsAtTime(int timeMs, out int currentLap)
        {
            var driverPositions = new List<(IDriver driver, TrackPosition position, RunningState runningState)>(NoOfDrivers);

            foreach (Driver driver in _driverModels.Keys)
            {
                TrackPosition trackPosition = _driverModels[driver].GetPositionAndRunningState(timeMs, out RunningState runningState);
                driverPositions.Add((driver, trackPosition, runningState));
            }

            currentLap = driverPositions.Where(tup => tup.runningState == RunningState.RUNNING).Max(tup => tup.position.Lap);

            // Sort positions into current race order
            driverPositions.Sort((tupa, tupb) => tupa.position.CompareTo(tupb.position));

            // TODO - tire compound changes

            // Build new driver standings

            var standings = new List<DriverStanding>(driverPositions.Count);

            int cumulativeGap = 0;

            for (int i = 0; i < driverPositions.Count; i++)
            {
                IDriver driver = driverPositions[i].driver;
                TrackPosition carPos = driverPositions[i].position;
                RunningState runningState = driverPositions[i].runningState;

                int gapToNextCar;

                // Lead car case
                if (i == 0)
                {
                    gapToNextCar = 0;
                }
                else
                {
                    TrackPosition nextCarPos = driverPositions[i - 1].position;

                    gapToNextCar = CalculateGap(carPos, nextCarPos);
                    cumulativeGap += gapToNextCar;
                }

                int racePos = i + 1;
                standings.Add(new DriverStanding(driver, racePos, cumulativeGap, gapToNextCar, carPos.LapDistanceFraction, carPos.Velocity, TireCompoundStore.SoftTyre, runningState));
            }

            return standings;
        }

        public bool TryGetCurrentLapForDriver(int currentTime, IDriver targetDriver, out int currentLap)
        {
            TrackPosition position = _driverModels[targetDriver].GetPositionAndRunningState(currentTime, out RunningState runningState);

            if (runningState == RunningState.RUNNING)
            {
                currentLap = position.Lap;
                return false;
            }
            currentLap = -1;
            return false;
        }

        private int CalculateGap(TrackPosition car, TrackPosition reference)
        {
            int lapDelta = reference.Lap - car.Lap;

            if (lapDelta == 0)
            {
                return (int)((reference.LapTimeFraction - car.LapTimeFraction) * reference.ForecastLapTime);
            }

            int gap = 0;

            if (lapDelta > 0)
            {
                // Add on delta from car to end of lap
                gap += (int)((1 - car.LapTimeFraction) * reference.ForecastLapTime);

                // Add on delta from start of lap to reference
                gap += reference.LapMs;
            }
            if (lapDelta > 1)
            {
                // Add on complete laps based on reference forecast
                gap += reference.ForecastLapTime * (lapDelta - 2);
            }

            return gap;
        }
    }
}
