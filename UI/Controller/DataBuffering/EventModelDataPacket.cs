using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.DataBuffering
{
    public class EventModelDataPacket : IEventModelDataPacket
    {
        public IList<IDriverStanding> Standings { get; }
        public int CurrentLap { get; }

        public bool WasCacheHit { get; set; }

        public ITrackState TrackState { get; }

        public EventModelDataPacket(IEnumerable<IDriverStanding> standings, int currentLap, ITrackState trackState, bool wasCacheHit)
        {
            Standings = standings.ToList();
            CurrentLap = currentLap;
            TrackState = trackState;
            WasCacheHit = wasCacheHit;
        }

        public override string ToString()
        {
            return $"Lap {CurrentLap}, Cache hit: {WasCacheHit}";
        }
    }
}
