namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IDriverModel
    {
        int DriverNoOfLaps { get; }
        int TotalNoOfLaps { get; }

        int DriverTotalTime { get; }

        TrackPosition GetPositionAndRunningState(int ms, out RunningState runningState);
    }
}
