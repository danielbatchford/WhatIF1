namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IDriverModel
    {
        int NoOfLaps { get; }

        int TotalTime { get; }

        bool TryGetPositionAtTime(int totalMs, out TrackPosition position);
    }
}
