using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class DriverModel : IDriverModel
    {
        private readonly double _trackLength;

        private readonly IList<int> _lapTimes;

        private readonly IList<IVelocityDistanceTimeContainer> _vdtContainers;

        public int NoOfLaps { get; }

        public int TotalTime { get; }

        public DriverModel(IEnumerable<int> lapTimes, IEnumerable<IVelocityDistanceTimeContainer> vdtContainers, double trackLength)
        {
            _lapTimes = lapTimes.ToList();

            _vdtContainers = vdtContainers.ToList();

            _trackLength = trackLength;

            NoOfLaps = _lapTimes.Count;

            TotalTime = _lapTimes.Sum();
        }

        public bool TryGetPositionAtTime(int totalMs, out TrackPosition position)
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

            position = new TrackPosition(totalMs, lapMs, lapIndex + 1, forecastLapTime, velocity, totalDistance, lapDistance, _trackLength);
            return true;
        }
    }
}
