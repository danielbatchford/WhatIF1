using System.Collections.Generic;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;

namespace WhatIfF1.Modelling.Events.TrackEventMarking
{
    public class MarkerComparer : IComparer<ITrackMarker>
    {
        public int Compare(ITrackMarker markerA, ITrackMarker markerB)
        {
            return markerA.StartMs.CompareTo(markerB.StartMs);
        }
    }
}
