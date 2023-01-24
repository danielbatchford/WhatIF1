using System;
using System.Collections.Generic;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;

namespace WhatIfF1.Modelling.Events.TrackEvents
{
    public sealed class TrackMarkerStore : ITrackMarkerStore
    {
        #region LazyInitialization

        public static TrackMarkerStore Instance => _lazy.Value;

        private readonly static Lazy<TrackMarkerStore> _lazy = new Lazy<TrackMarkerStore>(() => new TrackMarkerStore());

        #endregion LazyInitialization

        private readonly IDictionary<MarkerType, string> _markerDisplayNameDict;

        private readonly Color _redColor;
        private readonly Color _yellowColor;
        private readonly Color _greenColor;

        private TrackMarkerStore()
        {
            _markerDisplayNameDict = new Dictionary<MarkerType, string>
            {
                { MarkerType.OVERTAKE, "Overtake" },
                { MarkerType.PIT_STOP, "Pit Stop" },
                { MarkerType.RACE_WIN, "Race Win" },
                { MarkerType.RED_FLAG, "Red Flag" },
                { MarkerType.YELLOW_FLAG, "Yellow Flag" },
                { MarkerType.RETIREMENT, "Retirement" },
                { MarkerType.SAFETY_CAR, "Safety Car" },
                { MarkerType.VIRTUAL_SAFETY_CAR, "Virtual Safety Car" },
                { MarkerType.RACE_START, "Race Start" },
                { MarkerType.GREEN_FLAG, "Green Flag" }
            };

            _redColor = Color.FromRgb(238, 33, 9);
            _yellowColor = Color.FromRgb(240, 193, 9);
            _greenColor = Color.FromRgb(0, 208, 6);
        }

        public ITrackMarker CreatePitStopMarker(IDriver driver, int startMs, int endMs, int inLap)
        {
            return CreateRangedMarker(MarkerType.PIT_STOP, startMs, endMs, inLap, inLap + 1, driver.Color, driver);
        }

        public ITrackMarker CreateOvertakeMarker(IDriver driver, int ms, int lap)
        {
            return CreateInstantaneousMarker(MarkerType.OVERTAKE, ms, lap, driver.Color, driver);
        }

        public ITrackMarker CreateRaceWinMarker(IDriver driver, int ms, int lap)
        {
            return CreateInstantaneousMarker(MarkerType.RACE_WIN, ms, lap, driver.Color, driver);
        }

        public ITrackMarker CreateRetirementMarker(IDriver driver, int ms, int lap)
        {
            return CreateInstantaneousMarker(MarkerType.RETIREMENT, ms, lap, driver.Color, driver);
        }

        public ITrackMarker CreateRedFlagMarker(int ms, int lap)
        {
            return CreateInstantaneousMarker(MarkerType.RED_FLAG, ms, lap, _redColor);
        }

        public ITrackMarker CreateYellowFlagMarker(int startMs, int endMs, int startLap, int endLap)
        {
            return CreateRangedMarker(MarkerType.YELLOW_FLAG, startMs, endMs, startLap, endLap, _yellowColor);
        }

        public ITrackMarker CreateGreenFlagMarker(int ms, int lap)
        {
            return CreateInstantaneousMarker(MarkerType.GREEN_FLAG, ms, lap, _greenColor);
        }

        public ITrackMarker CreateSafetyCarMarker(int startMs, int endMs, int startLap, int endLap)
        {
            return CreateRangedMarker(MarkerType.SAFETY_CAR, startMs, endMs, startLap, endLap, _yellowColor);
        }

        public ITrackMarker CreateVirtualSafetyCarMarker(int startMs, int endMs, int startLap, int endLap)
        {
            return CreateRangedMarker(MarkerType.SAFETY_CAR, startMs, endMs, startLap, endLap, _yellowColor);
        }

        public ITrackMarker CreateRaceStartMarker()
        {
            return CreateInstantaneousMarker(MarkerType.RACE_START, 0, 1, _greenColor);
        }

        private ITrackMarker CreateRangedMarker(MarkerType eventType, int startMs, int endMs, int startLap, int endLap, Color color = default, IDriver driver = null)
        {
            string displayName = _markerDisplayNameDict[eventType];

            return new TrackMarker(displayName, eventType, startMs, endMs, startLap, endLap, color, driver);
        }

        private ITrackMarker CreateInstantaneousMarker(MarkerType eventType, int ms, int lap, Color color = default, IDriver driver = null)
        {
            string displayName = _markerDisplayNameDict[eventType];

            return new TrackMarker(displayName, eventType, ms, ms, lap, lap, color, driver);
        }
    }
}
