using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class DriverModel
    {
        public EventModel ParentModel { get; }

        private readonly double _trackLength;

        private readonly IList<int> _lapTimes;

        public int NoOfLaps { get; }

        public int TotalTime { get; }

        public DriverModel(EventModel parentModel, double trackLength, IEnumerable<int> lapTimes) 
        {
            ParentModel = parentModel;
            _trackLength = trackLength;

            _lapTimes = lapTimes.ToList();

            NoOfLaps = _lapTimes.Count;

            TotalTime = _lapTimes.Sum();
        }

        public Position GetPositionAtTime(int totalMs)
        {
            int lapIndex = 0;
            int lapMs = totalMs;

            while (lapMs > _lapTimes[lapIndex])
            {
                lapMs -= _lapTimes[lapIndex];
                lapIndex++;
            }

            // Interpolate the distance travelled around the lap based on the current time in the lap - TODO - correct model
            double lapDistance = NumberExtensions.InterpolateLinear(lapMs, 0, _lapTimes[lapIndex], 0, _trackLength);

            double totalDistance = lapIndex * _trackLength + lapDistance;

            return new Position(totalMs, lapMs, lapIndex, totalDistance, lapDistance, _trackLength);
        }
    }
}
