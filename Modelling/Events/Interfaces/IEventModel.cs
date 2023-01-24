using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.Modelling.Events.Interfaces
{
    public interface IEventModel
    {
        string Name { get; }
        int NoOfDrivers { get; }
        int TotalTime { get; }
        int NoOfLaps { get; }

        IEnumerable<IDriver> GetDrivers();
        IEnumerable<IDriverStanding> GetStandingsAtTime(int timeMs, out int currentLap);
        bool TryGetCurrentLapForDriver(int currentTime, IDriver targetDriver, out int currentLap);
    }
}
