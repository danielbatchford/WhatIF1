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
            if (!TryGetLapInfo(totalMs, out int lapMs, out int lapIndex))
            {
                // Implys car has retired or finished, cannot fetch lap data for lap greater than the laps travelled by this driver
                runningState = lapIndex == TotalNoOfLaps ? RunningState.FINISHED : RunningState.RETIRED;
                return _restingPosition;
            }

            _vdtContainers[lapIndex].GetLapDistanceAndVelocity(lapMs, out double lapDistance, out double velocity);

            double totalDistance = (lapIndex * _trackLength) + lapDistance;

            int forecastLapTime = _lapTimes[lapIndex];

            runningState = RunningState.RUNNING;
            int lap = lapIndex + 1;
            return new TrackPosition(totalMs, lapMs, lap, forecastLapTime, velocity, totalDistance, lapDistance, _trackLength);
        }

        private bool TryGetLapInfo(int totalMs, out int lapMs, out int lapIndex)
        {
            int idx = 0;
            int msCounter = totalMs;

            // Find the lap index based on the total time elapsed
            while (msCounter >= _lapTimes[idx])
            {
                msCounter -= _lapTimes[idx];
                idx++;

                // Implys car has retired or finished, cannot fetch lap data for lap greater than the laps travelled by this driver
                if (idx == DriverNoOfLaps)
                {
                    lapMs = default;
                    lapIndex = idx;
                    return false;
                }
            }

            lapMs = msCounter;
            lapIndex = idx;
            return true;

        }
    }
}
