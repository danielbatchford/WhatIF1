using WhatIfF1.Modelling.Events.Interfaces;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IEventModelDataProvider : IBufferableDataProvider<IEventModelDataPacket>
    {
        IEventModel Model { get; }
    }
}
