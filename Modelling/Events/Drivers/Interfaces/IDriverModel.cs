using System.Collections.Generic;
using WhatIfF1.Modelling.PitStops.Interfaces;

namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IDriverModel
    {
        int DriverNoOfLaps { get; }
        int TotalNoOfLaps { get; }
        int DriverTotalTime { get; }
        ITireCompound StartCompound { get; }

        IEnumerable<IPitStop> PitStops { get; }
        TrackPosition GetPositionAndRunningState(int ms, out RunningState runningState);
        ITireCompound GetCurrentTyreCompound(int lap);
        IVelocityDistanceTimeContainer GetVDTContainer(int lap);
    }
}
