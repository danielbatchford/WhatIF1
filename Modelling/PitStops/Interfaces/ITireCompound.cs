using System;
using System.Windows.Media;

namespace WhatIfF1.Modelling.PitStops.Interfaces
{
    public interface ITireCompound : IEquatable<ITireCompound>
    {
        string ScreenName { get; }
        string ImagePath { get; }
        Color ScreenColor { get; }

        /// <summary>
        /// e.g S, M or H
        /// </summary>
        char ScreenCharacter { get; }
    }
}
