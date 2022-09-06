using System;

namespace WhatIfF1.Modelling.Events
{
    public class Position
    {
        public int TotalMs { get; }

        public int LapMs { get; }

        public int Lap { get; }

        public double TotalDistance { get; }
        public double LapDistance { get; }

        public Position(int totalMs, int lapMs, int lap, double totalDistance, double lapDistance)
        {
            TotalMs = totalMs;
            LapMs = lapMs;
            Lap = lap;
            TotalDistance = totalDistance;
            LapDistance = lapDistance;
        }
    }
}
