using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Tires.Interfaces
{
    public interface IPitStopDataProvider : ITargetDriverDataProvider
    {
        ObservableRangeCollection<ITireRun> TireRuns { get; }

        ObservableRangeCollection<IPitStop> PitStops { get; }

        int NoOfStops { get; }

        int FastestStopMs { get; }
    }
}
