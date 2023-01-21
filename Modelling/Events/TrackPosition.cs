using System;

namespace WhatIfF1.Modelling.Events
{
    public sealed class TrackPosition : IEquatable<TrackPosition>, IComparable<TrackPosition>
    {
        public static TrackPosition OnStartLinePosition(int totalMs, int lap, double trackLength)
        {
            return new TrackPosition(totalMs, 0, lap, 0, 0, lap * trackLength, 0, trackLength);
        }

        public int TotalMs { get; }
        public int LapMs { get; }
        public int Lap { get; }
        public int ForecastLapTime { get; }
        public double Velocity { get; }
        public double TotalDistance { get; }
        public double LapDistance { get; }
        public double TrackLength { get; }
        public double LapDistanceFraction { get; }
        public double LapTimeFraction { get; }

        public TrackPosition(int totalMs, int lapMs, int lap, int forecastLapTime, double velocity, double totalDistance, double lapDistance, double trackLength)
        {
            TotalMs = totalMs;
            LapMs = lapMs;
            Lap = lap;
            ForecastLapTime = forecastLapTime;
            Velocity = velocity;
            TotalDistance = totalDistance;
            LapDistance = lapDistance;
            TrackLength = trackLength;

            LapDistanceFraction = lapDistance / trackLength;
            LapTimeFraction = ForecastLapTime > 0 ? (double)LapMs / ForecastLapTime : 0;
        }

        public override string ToString()
        {
            return $"Lap: {Lap}, Total Ms: {TotalMs}, Total Dist: {TotalDistance}, Lap Ms: {LapMs}, Lap Dist: {LapDistance}, Lap %: {LapDistanceFraction}, Forecast Lap Time: {ForecastLapTime}";
        }

        public bool Equals(TrackPosition other)
        {
            return Lap == other.Lap
                && LapMs == other.LapMs
                && TotalMs == other.TotalMs
                && ForecastLapTime == other.ForecastLapTime
                && Velocity == other.Velocity
                && TotalDistance == other.TotalDistance
                && LapDistance == other.LapDistance
                && TrackLength == other.TrackLength
                && LapDistanceFraction == other.LapDistanceFraction;
        }

        public int CompareTo(TrackPosition other)
        {
            return -Math.Sign(TotalDistance - other.TotalDistance);
        }
    }
}
