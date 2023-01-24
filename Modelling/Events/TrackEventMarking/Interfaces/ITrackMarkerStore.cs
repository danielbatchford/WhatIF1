using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.Modelling.Events.TrackEvents.Interfaces
{
    interface ITrackMarkerStore
    {
        ITrackMarker CreatePitStopMarker(IDriver driver, int startMs, int endMs, int inLap);

        ITrackMarker CreateOvertakeMarker(IDriver driver, int ms, int lap);

        ITrackMarker CreateRaceWinMarker(IDriver driver, int ms, int lap);

        ITrackMarker CreateRetirementMarker(IDriver driver, int ms, int lap);

        ITrackMarker CreateYellowFlagMarker(int startMs, int endMs, int startLap, int endLap);

        ITrackMarker CreateRedFlagMarker(int ms, int lap);

        ITrackMarker CreateGreenFlagMarker(int startMs, int lap);

        ITrackMarker CreateSafetyCarMarker(int startMs, int endMs, int startLap, int endLap);

        ITrackMarker CreateVirtualSafetyCarMarker(int startMs, int endMs, int startLap, int endLap);

        ITrackMarker CreateRaceStartMarker();
    }
}
