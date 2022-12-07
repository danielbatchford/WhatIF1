using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Util;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel : PropertyChangedWrapper
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
                if(_noOfLaps == value)
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

        public EventModel(string name, double trackLength, JArray driversJson, JArray lapTimesJson)
        {
            Name = name;

            // Fetch driver list from the response Json
            IEnumerable<Driver> drivers = Driver.GetDriverListFromJSON(driversJson);

            NumDrivers = drivers.Count();


            var lapTimes = new Dictionary<Driver, ICollection<int>>();

            // Initialise driver times dictionary
            foreach(Driver driver in drivers)
            {
                lapTimes.Add(driver, new List<int>());
            }

            const string timeFormat = @"m\:ss\.fff";

            foreach (JObject lapTimeJson in lapTimesJson)
            {
                foreach (Driver driver in drivers)
                {
                    foreach(var driverLapInnerJson in lapTimeJson["times"]["Timings"])
                    {
                        if (!driverLapInnerJson["driverId"].ToObject<string>().Equals(driver.DriverID))
                        {
                            continue;
                        }

                        string timingString = driverLapInnerJson["time"].ToObject<string>();

                        // Convert this timing string from format minute:second.millisecond to milliseconds
                        TimeSpan timeSpan = TimeSpan.ParseExact(timingString, timeFormat, CultureInfo.InvariantCulture);

                        lapTimes[driver].Add((int)timeSpan.TotalMilliseconds);

                        break;
                    }
                }
            }

            // Initialise driver models

            _driverModels = new Dictionary<Driver, DriverModel>(NumDrivers);

            foreach(Driver driver in drivers)
            {
                _driverModels.Add(driver, new DriverModel(this, trackLength, lapTimes[driver]));
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

        public bool TryGetPositionAtTime(Driver driver, int totalMs, out Position driverPos)
        {
            if(_driverModels[driver].TryGetPositionAtTime(totalMs, out driverPos))
            {
                return true;
            }

            return false;
        }
    }
}
