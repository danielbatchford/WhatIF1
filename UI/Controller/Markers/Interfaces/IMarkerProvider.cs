using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Markers.Interfaces
{
    public interface IMarkerProvider
    {
        SortedObservableRangeCollection<ITrackMarker> Markers { get; }

        void ClearMarkers(int startMs, int endMs);

        void ClearMarkers();
    }
}
