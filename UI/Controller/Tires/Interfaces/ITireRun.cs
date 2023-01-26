using WhatIfF1.Modelling.PitStops.Interfaces;

namespace WhatIfF1.UI.Controller.Tires.Interfaces
{
    public interface ITireRun
    {
        ITireCompound Tire { get; }

        int NoOfLaps { get; }

        int StartLap { get; }
    }
}
