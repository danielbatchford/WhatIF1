using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.UI.Controller.Tires.Interfaces;

namespace WhatIfF1.UI.Controller.Tires
{
    public class TireRun : ITireRun
    {
        public ITireCompound Tire { get; }
        public int StartLap { get; }
        public int NoOfLaps { get; }
        public string ScreenText { get; }

        public TireRun(ITireCompound tire, int startLap, int noOfLaps)
        {
            Tire = tire;
            StartLap = startLap;
            NoOfLaps = noOfLaps;
        }

        public override string ToString()
        {
            return $"{Tire} from lap {StartLap} for {NoOfLaps} laps";
        }
    }
}
