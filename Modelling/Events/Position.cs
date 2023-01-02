using System;

namespace WhatIfF1.Modelling.Events
{
    public sealed class Position : IEquatable<Position>, IComparable<Position>
    {
        public int TotalMs { get; }
        public int LapMs { get; }
        public int ForecastLapTime { get; }
        public int Lap { get; }
        public double TotalDistance { get; }
        public double LapDistance { get; }
        public double TrackLength { get; }
        public double PropOfLap { get; }

        public Position(int totalMs, int lapMs, int lap, int forecastLapTime, double lapFraction, double totalDistance, double lapDistance, double trackLength)
        {
            TotalMs = totalMs;
            LapMs = lapMs;
            ForecastLapTime = forecastLapTime;
            PropOfLap = lapFraction;
            Lap = lap;
            TotalDistance = totalDistance;
            LapDistance = lapDistance;
            TrackLength = trackLength;
        }

        public override string ToString()
        {
            return $"Lap: {Lap}, Total Ms: {TotalMs}, Total Dist: {TotalDistance}, Lap Ms: {LapMs}, Lap Dist: {LapDistance}, Lap %: {PropOfLap}, Forecast Lap Time: {ForecastLapTime}";
        }

        public bool Equals(Position other)
        {
            return Lap == other.Lap
                && LapMs == other.LapMs
                && TotalMs == other.TotalMs
                && ForecastLapTime == other.ForecastLapTime
                && TotalDistance == other.TotalDistance
                && LapDistance == other.LapDistance
                && TrackLength == other.TrackLength
                && PropOfLap == other.PropOfLap;
        }

        public int CompareTo(Position other)
        {
            return -Math.Sign(TotalDistance - other.TotalDistance);
        }
    }
}
