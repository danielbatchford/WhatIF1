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

        private double[] _distance;

        private bool _isIntegrated;

        public int[] Ms { get; }
        public double[] Velocity { get; }
        public int NumSamples { get; }

        public VelocityDistanceTimeContainer(int lap, double trackLength, IEnumerable<TelemetryTimeStamp> timeStamps)
        {
            _lap = lap;
            _trackLength = trackLength;

            Ms = timeStamps.Select(ts => ts.Ms).ToArray();
            Velocity = timeStamps.Select(ts => ts.Velocity).ToArray();
            NumSamples = Ms.Length;
        }

        public void GetLapDistanceAndVelocity(int lapMs, out double lapDistance, out double velocity)
        {
            if (!_isIntegrated)
            {
                IntegrateVelocity();
            }

            int closestIndex = Ms.FindClosestIndex(lapMs);
            int definedMs = Ms[closestIndex];

            if (definedMs == lapMs)
            {
                lapDistance = _distance[closestIndex];
                velocity = Velocity[closestIndex];
                return;
            }

            int upperIdx;
            int lowerIdx;

            if (closestIndex == 0)
            {
                upperIdx = 1;
                lowerIdx = 0;
            }
            else if (closestIndex == NumSamples - 1)
            {
                upperIdx = NumSamples - 1;
                lowerIdx = NumSamples - 2;
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
            double propAlongRange = (double)(lapMs - Ms[lowerIdx]) / (Ms[upperIdx] - Ms[lowerIdx]);
            lapDistance = _distance[lowerIdx] + (propAlongRange * (_distance[upperIdx] - _distance[lowerIdx]));
            velocity = Velocity[lowerIdx] + (propAlongRange * (Velocity[upperIdx] - Velocity[lowerIdx]));
        }

        private void IntegrateVelocity()
        {
            _distance = new double[NumSamples];

            _distance[0] = 0;

            for (int i = 1; i < NumSamples; i++)
            {
                double dt = (Ms[i] - Ms[i - 1]);
                _distance[i] = _distance[i - 1] + (0.5 * dt * ((Velocity[i - 1] + Velocity[i]) * _kphToMperMsFactor));
            }

            // Distance here needs to be scaled to the track length (as racing lines differ from track length)
            double scaleFactor = _trackLength / _distance[NumSamples - 1];

            for (int i = 0; i < NumSamples; i++)
            {
                _distance[i] *= scaleFactor;
            }

            _isIntegrated = true;
        }

        public override string ToString()
        {
            return $"Lap {_lap}, {NumSamples} samples";
        }
    }
}
