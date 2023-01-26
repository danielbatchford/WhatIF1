using System;
using System.Windows.Media;
using WhatIfF1.Util.Interfaces;

namespace WhatIfF1.Modelling.TrackStates.Interfaces
{
    public interface ITrackState : IMsRange, IEquatable<ITrackState>
    {
        int StartLap { get; }
        int EndLap { get; }
        FlagType Flag { get; }
        SafetyCarState SafetyCarState { get; }
        Color Color { get; }
        string SupportingTextA { get; }
        string SupportingTextB { get; }
    }
}