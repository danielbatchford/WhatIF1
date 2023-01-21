using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Telemetry;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class DriverModel
    {
        private readonly double _trackLength;

        private readonly IList<int> _lapTimes;

        private readonly IList<VelocityDistanceTimeContainer> _vdtContainers;

        public int NoOfLaps { get; }

        public int TotalTime { get; }

        public DriverModel(IEnumerable<int> lapTimes, IEnumerable<VelocityDistanceTimeContainer> vdtContainers, double trackLength)
        {
            _lapTimes = lapTimes.ToList();

            _vdtContainers = vdtContainers.ToList();

            _trackLength = trackLength;

            NoOfLaps = _lapTimes.Count;

            TotalTime = _lapTimes.Sum();
        }

        public bool TryGetPositionAtTime(int totalMs, out Position position)
        {
            int lapIndex = 0;
            int msCounter = totalMs;

            // Find the lap index based on the total time elapsed
            while (msCounter >= _lapTimes[lapIndex])
            {
                msCounter -= _lapTimes[lapIndex];
                lapIndex++;

                // Implys car has retired, cannot fetch position for lap greater than the laps travelled by this driver
                if (lapIndex == NoOfLaps)
                {
                    position = null;
                    return false;
                }
            }

            int lapMs = msCounter;

            _vdtContainers[lapIndex].GetLapDistanceAndVelocity(lapMs, out double lapDistance, out double velocity);

            double totalDistance = (lapIndex * _trackLength) + lapDistance;

            int forecastLapTime = _lapTimes[lapIndex];

            position = new Position(totalMs, lapMs, lapIndex + 1, forecastLapTime, velocity, totalDistance, lapDistance, _trackLength);
            return true;
        }
    }
}
