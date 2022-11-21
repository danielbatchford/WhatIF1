using System;

namespace WhatIfF1.Modelling.Events
{
    public sealed class Position
    {
        public int TotalMs { get; }
        public int LapMs { get; }
        public int Lap { get; }
        public double TotalDistance { get; }
        public double LapDistance { get; }
        public double TrackLength { get; }

        public Position(int totalMs, int lapMs, int lap, double totalDistance, double lapDistance, double trackLength)
        {
            TotalMs = totalMs;
            LapMs = lapMs;
            Lap = lap;
            TotalDistance = totalDistance;
            LapDistance = lapDistance;
            TrackLength = trackLength;
        }

        public override string ToString()
        {
            return $"Lap: {Lap}, Total Ms: {TotalMs}, Total Dist: {TotalDistance}, Lap Ms: {LapMs}, Lap Dist: {LapDistance}";
        }
    }
}
