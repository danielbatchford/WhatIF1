using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.Util.Events;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IEventModelDataProvider : IBufferableDataProvider<IEventModelDataPacket>, IDisposable
    {
        Task<int> GetCurrentLapForDriver(int ms, IDriver targetDriver);
        IEnumerable<ITrackState> GetTrackStates();
        Task<int> GetTotalLapsForDriver(IDriver driver);
        Task<IVelocityDistanceTimeContainer> GetVDTContainer(IDriver driver, int driverLap);
        Task<(ITireCompound startCompound, IEnumerable<IPitStop> stops)> GetPitStopsForDriver(IDriver driver);

        event ItemChangedEventHandler<int> TotalTimeChanged;
        event ItemChangedEventHandler<int> NoOfLapsChanged;

    }
}
