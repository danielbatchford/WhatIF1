using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util.Events;

namespace WhatIfF1.Modelling.Events.Interfaces
{
    public interface IEventModel
    {
        string Name { get; }
        int NoOfDrivers { get; }
        int TotalTime { get; }
        int NoOfLaps { get; }

        IEnumerable<IDriver> GetDrivers();
        IEnumerable<ITrackState> GetTrackStates();
        IEnumerable<IDriverStanding> GetStandingsAtTime(int timeMs, out int currentLap, out ITrackState trackState);
        bool TryGetCurrentLapForDriver(int currentTime, IDriver targetDriver, out int currentLap);

        event ItemChangedEventHandler<int> TotalTimeChanged;

        event ItemChangedEventHandler<int> NoOfLapsChanged;

    }
}
