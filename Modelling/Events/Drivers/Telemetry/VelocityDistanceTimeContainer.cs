using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public class VelocityDistanceTimeContainer : IVelocityDistanceTimeContainer
    {
        /// <summary>
        /// Factor for converting kph to meters per millisecond
        /// </summary>
        private const double _kphToMperMsFactor = 0.000277778;

        private readonly int _lap;

        private readonly double _trackLength;

        private readonly int[] _ms;

        private readonly double[] _velocity;

        private double[] _distance;

        private readonly int _n;

        private bool _isIntegrated;

        public VelocityDistanceTimeContainer(int lap, double trackLength, IEnumerable<TelemetryTimeStamp> timeStamps)
        {
            _lap = lap;
            _trackLength = trackLength;

            _ms = timeStamps.Select(ts => ts.Ms).ToArray();
            _velocity = timeStamps.Select(ts => ts.Velocity).ToArray();
            _n = _ms.Length;
        }

        public void GetLapDistanceAndVelocity(int lapMs, out double lapDistance, out double velocity)
        {
            if (!_isIntegrated)
            {
                IntegrateVelocity();
            }

            int closestIndex = _ms.FindClosestIndex(lapMs);
            int definedMs = _ms[closestIndex];

            if (definedMs == lapMs)
            {
                lapDistance = _distance[closestIndex];
                velocity = _velocity[closestIndex];
                return;
            }

            int upperIdx;
            int lowerIdx;

            if (closestIndex == 0)
            {
                upperIdx = 1;
                lowerIdx = 0;
            }
            else if (closestIndex == _n - 1)
            {
                upperIdx = _n - 1;
                lowerIdx = _n - 2;
            }
            else if (definedMs > lapMs)
            {
                upperIdx = closestIndex;
                lowerIdx = closestIndex - 1;
            }
            else
            {
                upperIdx = closestIndex + 1;
                lowerIdx = closestIndex;
            }

            // Linear interpolate distance based on requested ms and bordering ms
            double propAlongRange = (double)(lapMs - _ms[lowerIdx]) / (_ms[upperIdx] - _ms[lowerIdx]);
            lapDistance = _distance[lowerIdx] + (propAlongRange * (_distance[upperIdx] - _distance[lowerIdx]));
            velocity = _velocity[lowerIdx] + (propAlongRange * (_velocity[upperIdx] - _velocity[lowerIdx]));
        }

        private void IntegrateVelocity()
        {
            _distance = new double[_n];

            _distance[0] = 0;

            for (int i = 1; i < _n; i++)
            {
                double dt = (_ms[i] - _ms[i - 1]);
                _distance[i] = _distance[i - 1] + (0.5 * dt * ((_velocity[i - 1] + _velocity[i]) * _kphToMperMsFactor));
            }

            // Distance here needs to be scaled to the track length (as racing lines differ from track length)
            double scaleFactor = _trackLength / _distance[_n - 1];

            for (int i = 0; i < _n; i++)
            {
                _distance[i] *= scaleFactor;
            }

            _isIntegrated = true;
        }

        public override string ToString()
        {
            return $"Lap {_lap}, {_n} samples";
        }
    }
}
