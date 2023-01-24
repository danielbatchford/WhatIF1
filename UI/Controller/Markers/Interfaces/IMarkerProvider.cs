using System.Collections.ObjectModel;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;

namespace WhatIfF1.UI.Controller.Markers.Interfaces
{
    public interface IMarkerProvider
    {
        ObservableCollection<ITrackMarker> Markers { get; }

        void BeginMarkerLoading();

        void PauseMarkerLoading();

        void InvalidateMarkers(int startMs, int endMs);
    }
}
