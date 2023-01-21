using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Events.Drivers.Telemetry;
using WhatIfF1.Modelling.Tires;
using WhatIfF1.UI.Controller;
using WhatIfF1.Util;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel : NotifyPropertyChangedWrapper
    {
        private readonly IDictionary<Driver, DriverModel> _driverModels;

        public int NumDrivers { get; }

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

        public EventModel(string name, double trackLength, JArray driversJson, JArray lapTimesJson, JObject telemetryJson)
        {
            Name = name;

            // Fetch driver list from the response Json
            IEnumerable<Driver> drivers = Driver.GetDriverListFromJSON(driversJson);

            NumDrivers = drivers.Count();

            var lapTimes = new Dictionary<Driver, ICollection<int>>();

            // Initialise driver times dictionary
            foreach (Driver driver in drivers)
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

            // Initialise driver models

            _driverModels = new Dictionary<Driver, DriverModel>(NumDrivers);

            foreach (Driver driver in drivers)
            {
                _driverModels.Add(driver, new DriverModel(lapTimes[driver], vdtContainers[driver], trackLength));
            }

            // Initialise number of laps as the max of the laps completed by each driver
            NoOfLaps = _driverModels.Values.Max(dModel => dModel.NoOfLaps);

            // Initialse the total time as the maximum time of the total times of each driver
            TotalTime = _driverModels.Values.Max(dMOdel => dMOdel.TotalTime);
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<Driver> GetDrivers()
        {
            return new List<Driver>(_driverModels.Keys);
        }

        public IEnumerable<DriverStanding> GetStandingsAtTime(int timeMs, out int currentLap)
        {
            var driverPositions = new List<(Driver driver, Position position)>();

            foreach (Driver driver in _driverModels.Keys)
            {
                if (_driverModels[driver].TryGetPositionAtTime(timeMs, out Position driverPos))
                {
                    driverPositions.Add((a: driver, driverPos));
                }
            }

            currentLap = driverPositions.Max(tup => tup.position.Lap);

            // Sort positions into current race order
            driverPositions.Sort((tupa, tupb) =>
            {
                Position a = tupa.position;
                Position b = tupb.position;

                return a.CompareTo(b);
            });

            // TODO - tire compound changes

            // Build new driver standings

            var standings = new List<DriverStanding>(driverPositions.Count);

            Position leadCarPos = driverPositions[0].position;

            for (int i = 0; i < driverPositions.Count; i++)
            {
                Driver driver = driverPositions[i].driver;
                Position carPos = driverPositions[i].position;

                int gapToNextCar;
                int gapToLead;

                // Lead car case
                if (i == 0)
                {
                    gapToNextCar = 0;
                    gapToLead = 0;
                }
                else
                {
                    Position nextCarPos = driverPositions[i - 1].position;

                    gapToNextCar = CalculateGap(carPos, nextCarPos);
                    gapToLead = CalculateGap(carPos, leadCarPos);
                }

                int racePos = i + 1;

                standings.Add(new DriverStanding(driver, racePos, gapToLead, gapToNextCar, carPos.LapDistanceFraction, carPos.Velocity, TireCompoundStore.SoftTyre));
            }

            return standings;
        }

        public int GetCurrentLap(int timeMs, Driver driver = null)
        {
            var positions = new List<Position>();

            var driversInLapEval = driver is null ? _driverModels.Keys : new List<Driver> { driver };

            foreach (Driver driverInEval in driversInLapEval)
            {
                if (_driverModels[driverInEval].TryGetPositionAtTime(timeMs, out Position driverPos))
                {
                    positions.Add(driverPos);
                }
            }

            return positions.Max(pos => pos.Lap);
        }

        private int CalculateGap(Position car, Position reference)
        {
            int lapDelta = reference.Lap - car.Lap;

            if (lapDelta == 0)
            {
                // Return time based off reference forecast and proportion of lap between cars
                return (int)((reference.LapDistanceFraction - car.LapDistanceFraction) * reference.ForecastLapTime);
            }

            int gap = 0;

            if (lapDelta > 0)
            {
                // Add on delta from car to end of lap
                gap += (int)((1 - car.LapDistanceFraction) * reference.ForecastLapTime);

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
