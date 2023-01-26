using System;

namespace WhatIfF1.Modelling.PitStops.Interfaces
{
    public interface IPitStop : IEquatable<IPitStop>
    {
        ITireCompound OldCompound { get; }
        ITireCompound NewCompound { get; }
        int InLap { get; }
        int OutLap { get; }
        int StopNumber { get; }
        int PitTime { get; }
    }
}
