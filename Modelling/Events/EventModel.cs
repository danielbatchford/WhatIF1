using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tracks;

namespace WhatIfF1.Modelling.Events
{
    public class EventModel
    {
        private readonly double _trackLength;

        private readonly IDictionary<Driver, DriverModel> _driverModels;

        private readonly int _numDrivers;

        public string Name { get; }
        public int NumLaps { get; }

        public EventModel(Track track, int year, JArray json)
        {
            _trackLength = track.TrackLength;

            Console.WriteLine(json);

            // Calculate the number of laps completed as the minimum number of laps where the driver's status is "finished"
            NumLaps = json
                .Where(driver => driver["status"].ToObject<string>().Equals("Finished"))
                .Min(driver => driver["laps"].ToObject<int>());

            Name = $"{track} - {year} - {NumLaps} Laps";

            // Fetch driver list from the response Json
            IEnumerable<Driver> drivers = Driver.GetDriverListFromJSON(json);

            _numDrivers = drivers.Count();

            _driverModels = new Dictionary<Driver, DriverModel>(_numDrivers);

            foreach(Driver driver in drivers)
            {
                _driverModels.Add(driver, new DriverModel(this));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
