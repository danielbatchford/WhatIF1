using System;
using System.Collections.ObjectModel;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.TrackMaps.Interfaces;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IEventController : IDisposable, IPlayable
    {
        ITrackMapProvider MapProvider { get; }

        IGraphProvider GraphProvider { get; }

        IDriverStanding SelectedStanding { get; }

        IEventModelDataProvider DataProvider { get; }

        int CurrentLap { get; set; }

        int CurrentTime { get; set; }

        ObservableCollection<IDriverStanding> Standings { get; }
    }
}
