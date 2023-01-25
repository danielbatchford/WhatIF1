using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.Util.Events;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IEventModelDataProvider : IBufferableDataProvider<IEventModelDataPacket>, IDisposable
    {
        Task<int> GetCurrentLapForDriver(int ms, IDriver targetDriver);
        IEnumerable<ITrackState> GetTrackStates();

        event ItemChangedEventHandler<int> TotalTimeChanged;
        event ItemChangedEventHandler<int> NoOfLapsChanged;

    }
}
