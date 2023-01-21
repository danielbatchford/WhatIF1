using System;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Tires.Interfaces;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IDriverStanding : IEquatable<IDriverStanding>
    {
        IDriver Driver { get; set; }
        int RacePosition { get; set; }
        int GapToLead { get; set; }
        int GapToNextCar { get; set; }
        double ProportionOfLap { get; set; }
        double Velocity { get; set; }
        ITireCompound TireCompound { get; set; }
        RunningState State { get; set; }
        string TimingScreenText { get; set; }
        double TimingScreenTextOpacity { get; set; }

        void UpdateFromOtherStanding(IDriverStanding standing);
    }
}
