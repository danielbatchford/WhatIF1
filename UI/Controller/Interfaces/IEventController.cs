using System;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.TrackMaps.Interfaces;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IEventController : IDisposable, IPlayable
    {
        IEventModel Model { get; set; }

        ITrackMapProvider MapProvider { get; }

        IGraphProvider GraphProvider { get; }

        IDriverStanding SelectedStanding { get; }

        int CurrentLap { get; set; }

        ObservableRangeCollection<IDriverStanding> Standings { get; }
    }
}
