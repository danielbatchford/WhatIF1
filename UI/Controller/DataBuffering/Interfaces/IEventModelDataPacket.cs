using System.Collections.Generic;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IEventModelDataPacket : IBufferedDataPacket
    {
        IList<IDriverStanding> Standings { get; }
        int CurrentLap { get; }

        ITrackState TrackState { get; }
    }
}
