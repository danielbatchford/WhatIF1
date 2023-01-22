using System.Collections.Generic;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.DataBuffering
{
    public class EventModelDataPacket : IEventModelDataPacket
    {
        public IEnumerable<IDriverStanding> Standings { get; }
        public int CurrentLap { get; }

        public bool WasCacheHit { get; set; }

        public EventModelDataPacket(IEnumerable<IDriverStanding> standings, int currentLap, bool wasCacheHit)
        {
            Standings = standings;
            CurrentLap = currentLap;
            WasCacheHit = wasCacheHit;
        }

        public override string ToString()
        {
            return $"Lap {CurrentLap}, Cache hit: {WasCacheHit}";
        }
    }
}
