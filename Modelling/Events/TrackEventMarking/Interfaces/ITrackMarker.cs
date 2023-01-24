﻿using System;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.Modelling.Events.TrackEvents.Interfaces
{
    public interface ITrackMarker : IEquatable<ITrackMarker>
    {
        string DisplayName { get; }
        MarkerType MarkerType { get; }
        int StartMs { get; }
        int EndMs { get; }
        int StartLap { get; }
        int EndLap { get; }
        string SupportingTextA { get; }
        string SupportingTextB { get; }
        string TrackMarkerIconFilePath { get; }
        IDriver Driver { get; }
        Color Color { get; }
    }
}
