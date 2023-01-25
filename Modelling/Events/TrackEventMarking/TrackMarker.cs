using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.TrackEvents
{
    public class TrackMarker : ITrackMarker
    {
        private static readonly IDictionary<MarkerType, string> _staticMarkerTypeToIconFileNameDict = new Dictionary<MarkerType, string>
            {
                { MarkerType.RED_FLAG, "red_flag.png" },
                { MarkerType.YELLOW_FLAG, "yellow_flag.png" },
                { MarkerType.GREEN_FLAG, "green_flag.png" },
                { MarkerType.SAFETY_CAR, "safety_car.png" },
                { MarkerType.VIRTUAL_SAFETY_CAR, "virtual_safety_car.png" },
                { MarkerType.RACE_START, "race_start.png" }
            };

        public string DisplayName { get; }

        public MarkerType MarkerType { get; }

        public int StartMs { get; }

        public int EndMs { get; }

        public int StartLap { get; }

        public int EndLap { get; }

        public string SupportingTextA { get; }

        public string SupportingTextB { get; }

        public string TrackMarkerIconFilePath { get; }

        public Color Color { get; }

        public IDriver Driver { get; }

        public TrackMarker(string displayName, MarkerType markerType, int startMs, int endMs, int startLap, int endLap, Color color, IDriver driver = null)
        {
            if (startMs > endMs)
            {
                throw new ArgumentException("Provided track marker start ms exceeded end ms");
            }

            if (startLap > endLap)
            {
                throw new ArgumentException("Provided track marker start lap exceeded end lap");
            }

            DisplayName = displayName;
            MarkerType = markerType;
            StartMs = startMs;
            EndMs = endMs;
            StartLap = startLap;
            EndLap = endLap;
            Color = color;
            Driver = driver;

            SupportingTextA = GetSupportingTextA();
            SupportingTextB = GetSupportingTextB();

            if (Driver == null)
            {
                string iconFileName = _staticMarkerTypeToIconFileNameDict[markerType];
                string eventIconsFolder = FileAdapter.Instance.TrackMarkerIconsRoot;
                TrackMarkerIconFilePath = Path.Combine(eventIconsFolder, iconFileName);
            }
            else
            {
                TrackMarkerIconFilePath = Driver.ImagePath;
            }
        }

        public override string ToString()
        {
            if (StartLap - EndLap == 0)
            {
                return $"{ DisplayName} - lap {StartLap} to lap {EndLap}, from {StartMs}ms to {EndLap}ms";
            }
            else
            {
                return $"{ DisplayName} - lap {StartLap}, from {StartMs}ms to {EndLap}ms";
            }
        }

        public bool Equals(ITrackMarker iMarker)
        {
            return iMarker is TrackMarker marker &&
                   DisplayName == marker.DisplayName &&
                   MarkerType == marker.MarkerType &&
                   StartMs == marker.StartMs &&
                   EndMs == marker.EndMs &&
                   StartLap == marker.StartLap &&
                   EndLap == marker.EndLap &&
                   TrackMarkerIconFilePath == marker.TrackMarkerIconFilePath &&
                   EqualityComparer<IDriver>.Default.Equals(Driver, marker.Driver);
        }

        private string GetSupportingTextA()
        {
            switch (MarkerType)
            {
                case MarkerType.PIT_STOP:
                case MarkerType.OVERTAKE:
                case MarkerType.RETIREMENT:
                case MarkerType.RACE_WIN:
                    return Driver.DriverLetters;

                case MarkerType.RED_FLAG:
                case MarkerType.GREEN_FLAG:
                    return $"At {StringExtensions.ToF1TimingScreenFormat(StartMs)}";

                case MarkerType.YELLOW_FLAG:
                case MarkerType.SAFETY_CAR:
                case MarkerType.VIRTUAL_SAFETY_CAR:

                    TimeSpan duration = new TimeSpan(10000 * (EndMs - StartMs));
                    return $"Duration: {Math.Round(duration.TotalSeconds, 0)}s";

                case MarkerType.RACE_START:
                    return string.Empty;
            }

            throw new NotImplementedException($"No supporting text A implementation for marker type {MarkerType}");
        }

        private string GetSupportingTextB()
        {
            switch (MarkerType)
            {
                case MarkerType.PIT_STOP:
                case MarkerType.OVERTAKE:
                case MarkerType.RETIREMENT:
                case MarkerType.RACE_WIN:
                case MarkerType.GREEN_FLAG:
                case MarkerType.RED_FLAG:
                    return $"Lap {StartLap}";

                case MarkerType.YELLOW_FLAG:
                case MarkerType.SAFETY_CAR:
                case MarkerType.VIRTUAL_SAFETY_CAR:
                    return $"Laps {StartLap}-{EndLap}";

                case MarkerType.RACE_START:
                    return string.Empty;
            }

            throw new NotImplementedException($"No supporting text B implementation for marker type {MarkerType}");
        }

    }
}
