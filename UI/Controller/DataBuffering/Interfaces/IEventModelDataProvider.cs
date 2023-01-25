using System;
using System.Threading.Tasks;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Util.Events;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IEventModelDataProvider : IBufferableDataProvider<IEventModelDataPacket>, IDisposable
    {
        event ItemChangedEventHandler<int> TotalTimeChanged;
        event ItemChangedEventHandler<int> NoOfLapsChanged;

        Task<int> GetCurrentLapForDriver(int currentTime, IDriver targetDriver);
    }
}
