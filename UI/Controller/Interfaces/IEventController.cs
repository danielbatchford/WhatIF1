using System;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.TrackMaps.Interfaces;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IEventController : IDisposable, IPlayable
    {
        ITrackMapProvider MapProvider { get; }

        IGraphProvider GraphProvider { get; }

        IDriverStanding SelectedStanding { get; }

        IEventModelDataProvider DataProvider { get; }

        ITrackState CurrentTrackState { get; set; }

        int CurrentLap { get; set; }

        int NoOfLaps { get; set; }

        ObservableRangeCollection<IDriverStanding> Standings { get; }
    }
}
