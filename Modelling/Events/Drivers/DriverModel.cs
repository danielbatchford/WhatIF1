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

        /// <summary>
        /// Either where a position over the finishing line, or the position where the car retired
        /// </summary>
        private readonly TrackPosition _restingPosition;

        public int DriverNoOfLaps { get; }

        public int TotalNoOfLaps { get; }

        public int DriverTotalTime { get; }

        public DriverModel(IEnumerable<int> lapTimes, IEnumerable<IVelocityDistanceTimeContainer> vdtContainers, double trackLength, int totalNoOfLaps, bool isDriverRetired)
        {
            _lapTimes = lapTimes.ToList();

            _vdtContainers = vdtContainers.ToList();

            _trackLength = trackLength;

            DriverNoOfLaps = _lapTimes.Count;
            TotalNoOfLaps = totalNoOfLaps;

            DriverTotalTime = _lapTimes.Sum();

            if (isDriverRetired)
            {
                _restingPosition = TrackPosition.OnStartLinePosition(DriverTotalTime, DriverNoOfLaps, trackLength);
            }
            else
            {
                _restingPosition = TrackPosition.OnStartLinePosition(DriverTotalTime, DriverNoOfLaps + 1, trackLength);
            }
        }

        public TrackPosition GetPositionAndRunningState(int totalMs, out RunningState runningState)
        {
            int lapIndex = 0;
            int msCounter = totalMs;

            // Find the lap index based on the total time elapsed
            while (msCounter >= _lapTimes[lapIndex])
            {
                msCounter -= _lapTimes[lapIndex];
                lapIndex++;

                // Implys car has retired or finished, cannot fetch position for lap greater than the laps travelled by this driver
                if (lapIndex == DriverNoOfLaps)
                {
                    runningState = lapIndex == TotalNoOfLaps ? RunningState.FINISHED : RunningState.RETIRED;
                    return _restingPosition;
                }
            }

            int lapMs = msCounter;

            _vdtContainers[lapIndex].GetLapDistanceAndVelocity(lapMs, out double lapDistance, out double velocity);

            double totalDistance = (lapIndex * _trackLength) + lapDistance;

            int forecastLapTime = _lapTimes[lapIndex];

            runningState = RunningState.RUNNING;
            return new TrackPosition(totalMs, lapMs, lapIndex + 1, forecastLapTime, velocity, totalDistance, lapDistance, _trackLength);
        }
    }
}
