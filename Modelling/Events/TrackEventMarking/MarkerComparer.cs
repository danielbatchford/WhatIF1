using System.Collections.Generic;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;

namespace WhatIfF1.Modelling.Events.TrackEventMarking
{
    public class MarkerComparer : IComparer<ITrackMarker>
    {
        public int Compare(ITrackMarker markerA, ITrackMarker markerB)
        {
            int sign = markerA.StartMs.CompareTo(markerB.StartMs);

            if (sign == 0)
            {
                // Return shorter duration markers first
                return markerA.EndMs.CompareTo(markerB.EndMs);
            }

            return sign;
        }
    }
}
