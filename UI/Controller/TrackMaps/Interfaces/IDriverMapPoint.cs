using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.UI.Controller.TrackMaps.Interfaces
{
    public interface IDriverMapPoint : IMapPoint
    {
        IDriver Driver { get; }

        bool IsNotRunning { get; set; }
    }
}
