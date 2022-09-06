using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel
    {
        private readonly IDictionary<Driver, DriverModel> _driverModels;

        public int NumDrivers { get; }

        public string Name { get; }

        public EventModel(string name, double trackLength, int year, JArray driversJson, JArray lapTimesJson)
        {
            Name = name;

            // Fetch driver list from the response Json
            IEnumerable<Driver> drivers = Driver.GetDriverListFromJSON(driversJson);

            NumDrivers = drivers.Count();

            _driverModels = new Dictionary<Driver, DriverModel>(NumDrivers);

            foreach(Driver driver in drivers)
            {
                _driverModels.Add(driver, new DriverModel(this, trackLength, lapTimesJson));
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<Driver> GetDrivers()
        {
            return new List<Driver>(_driverModels.Keys);
        }

        public Position GetPositionAtTime(Driver driver, int totalMs)
        {
            return _driverModels[driver].GetPositionAtTime(totalMs);
        }
    }
}
