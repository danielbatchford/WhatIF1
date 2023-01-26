using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.Drivers.Telemetry;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.Modelling.PitStops;
using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Events;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel : NotifyPropertyChangedWrapper, IEventModel
    {
        #region Static

        public static EventModel GetModel(string name, double trackLength, JArray driversJson, JArray lapTimesJson, JArray pitStopsJson, JObject telemetryJson)
        {
            var drivers = Driver.GetDriversAndRetirementsListFromJSON(driversJson, out IDictionary<IDriver, bool> isDriverRetiredDict);
            int noOfDrivers = drivers.Count();

            var lapTimes = ParseLapTimes(drivers, lapTimesJson);

            var telemetryParser = new TelemetryParser(trackLength);

            var vdtContainers = telemetryParser.ParseTelemetryJson(drivers, lapTimes, telemetryJson);

            var tireCompounds = ParsePitStops(drivers, pitStopsJson, out IDictionary<IDriver, ITireCompound> startingCompounds);

            // Initialise number of laps as the max of the laps completed by each driver
            int noOfLaps = lapTimes.Values.Max(lt => lt.Count);

            // Initialise driver models

            var driverModels = new Dictionary<IDriver, IDriverModel>(noOfDrivers);

            foreach (IDriver driver in drivers)
            {
                driverModels.Add(driver, new DriverModel(
                    lapTimes[driver],
                    vdtContainers[driver],
                    tireCompounds[driver],
                    startingCompounds[driver],
                    trackLength,
                    noOfLaps,
                    isDriverRetiredDict[driver]));
            }

            // Initialse the total time as 1ms before the maximum time of the total times of each driver
            int totalTime = driverModels.Values.Max(dMOdel => dMOdel.DriverTotalTime) - 1;

            var trackStates = CalculateTrackStates(driverModels);

            return new EventModel(name, noOfDrivers, noOfLaps, totalTime, driverModels, trackStates);
        }

        private static IDictionary<IDriver, ICollection<IPitStop>> ParsePitStops(IEnumerable<IDriver> drivers, JArray json, out IDictionary<IDriver, ITireCompound> startingCompounds)
        {
            var pitStops = new Dictionary<IDriver, ICollection<IPitStop>>();
            startingCompounds = new Dictionary<IDriver, ITireCompound>();

            // Initialise driver pit stops dictionary and starting compounds dictionary
            foreach (IDriver driver in drivers)
            {
                pitStops.Add(driver, new List<IPitStop>());
            }

            string[] timeFormats = new string[]
            {
                @"s\.fff",
                @"ss\.fff",
                @"sss\.fff",
            };

            foreach (IDriver driver in drivers)
            {
                foreach (var pitJson in json.Where(ps => ((string)ps["driverId"]).Equals(driver.DriverID)))
                {
                    int stopNumber = (int)pitJson["stop"];
                    int inLap = (int)pitJson["lap"];
                    int outLap = inLap + 1;

                    string timingString = pitJson["duration"].ToObject<string>();

                    // TODO - this
                    var oldCompound = TireCompoundStore.GetRandomCompound();
                    var newCompound = TireCompoundStore.GetRandomCompound();

                    // Convert this timing string from format second.millisecond to milliseconds
                    TimeSpan pitTime = TimeSpan.ParseExact(timingString, timeFormats, CultureInfo.InvariantCulture);

                    pitStops[driver].Add(new PitStop(stopNumber, (int)pitTime.TotalMilliseconds, inLap, outLap, oldCompound, newCompound));
                }

                // TODO - this
                startingCompounds.Add(driver, TireCompoundStore.GetRandomCompound());
            }

            return pitStops;
        }

        private static IDictionary<IDriver, ICollection<int>> ParseLapTimes(IEnumerable<IDriver> drivers, JArray json)
        {
            var lapTimes = new Dictionary<IDriver, ICollection<int>>();

            // Initialise driver times dictionary
            foreach (IDriver driver in drivers)
            {
                lapTimes.Add(driver, new List<int>());
            }

            const string timeFormat = @"m\:ss\.fff";

            foreach (JObject lapTimeJson in json)
            {
                foreach (Driver driver in drivers)
                {
                    // If no element is found, this implies the driver has retired from the race
                    JToken timingObject = lapTimeJson["Timings"].SingleOrDefault(timing => ((string)timing["driverId"]).Equals(driver.DriverID));

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

            return lapTimes;
        }

        private static IEnumerable<ITrackState> CalculateTrackStates(IDictionary<IDriver, IDriverModel> driverModels)
        {
            // TODO - this
            return new List<ITrackState>
            {
                new TrackState(0, 1000000000, 1, 10, TrackStates.FlagType.GREEN, TrackStates.SafetyCarState.NONE)
            };
        }

        #endregion Static

        private readonly IDictionary<IDriver, IDriverModel> _driverModels;
        private readonly IList<ITrackState> _trackStates;

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

                NoOfLapsChanged?.Invoke(this, new ItemChangedEventArgs<int>(_noOfLaps, value));
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

                TotalTimeChanged?.Invoke(this, new ItemChangedEventArgs<int>(_totalTime, value));
                _totalTime = value;
                OnPropertyChanged();
            }
        }

        private EventModel(string name, int noOfDrivers, int noOfLaps, int totalTime, IDictionary<IDriver, IDriverModel> driverModels, IEnumerable<ITrackState> trackStates)
        {
            Name = name;
            NoOfDrivers = noOfDrivers;

            NoOfLaps = noOfLaps;
            TotalTime = totalTime;

            _driverModels = driverModels;
            _trackStates = trackStates.ToList();
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<IDriver> GetDrivers()
        {
            return new List<IDriver>(_driverModels.Keys);
        }

        public IEnumerable<ITrackState> GetTrackStates()
        {
            return _trackStates;
        }

        public IVelocityDistanceTimeContainer GetVDTContainer(IDriver driver, int lap)
        {
            return _driverModels[driver].GetVDTContainer(lap);
        }

        public int GetTotalLapsForDriver(IDriver driver)
        {
            return _driverModels[driver].DriverNoOfLaps;
        }

        public IEnumerable<IDriverStanding> GetStandingsAtTime(int ms, out int currentLap, out ITrackState trackState)
        {
            var driverPositions = new List<(IDriver driver, TrackPosition position, RunningState runningState)>(NoOfDrivers);

            foreach (Driver driver in _driverModels.Keys)
            {
                TrackPosition trackPosition = _driverModels[driver].GetPositionAndRunningState(ms, out RunningState runningState);
                driverPositions.Add((driver, trackPosition, runningState));
            }

            currentLap = driverPositions.Where(tup => tup.runningState == RunningState.RUNNING).Max(tup => tup.position.Lap);

            trackState = GetTrackState(ms);

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

                ITireCompound tireCompound = _driverModels[driver].GetCurrentTyreCompound(carPos.Lap);

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
                standings.Add(new DriverStanding(driver, racePos, cumulativeGap, gapToNextCar, carPos.LapDistance, carPos.LapDistanceFraction, carPos.Velocity, tireCompound, runningState));
            }

            return standings;
        }

        public bool TryGetCurrentLapForDriver(int ms, IDriver targetDriver, out int currentLap)
        {
            TrackPosition position = _driverModels[targetDriver].GetPositionAndRunningState(ms, out RunningState runningState);

            if (runningState == RunningState.RUNNING)
            {
                currentLap = position.Lap;
                return false;
            }
            currentLap = -1;
            return false;
        }

        public IEnumerable<IPitStop> GetPitStopsForDriver(IDriver driver, out ITireCompound startCompound)
        {
            startCompound = _driverModels[driver].StartCompound;
            return _driverModels[driver].PitStops;
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

        private ITrackState GetTrackState(int ms)
        {
            foreach (var trackState in _trackStates)
            {
                if (trackState.StartMs <= ms && ms <= trackState.EndMs)
                {
                    return trackState;
                }
            }

            throw new Exception($"No track state found at millisecond {ms}");
        }

        public event ItemChangedEventHandler<int> TotalTimeChanged;
        public event ItemChangedEventHandler<int> NoOfLapsChanged;
    }
}
