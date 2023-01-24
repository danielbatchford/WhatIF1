using System;
using System.Windows.Media;

namespace WhatIfF1.Modelling.Tires.Interfaces
{
    public interface ITireCompound : IEquatable<ITireCompound>
    {
        string ScreenName { get; }
        Color ScreenColor { get; }

        /// <summary>
        /// e.g S, M or H
        /// </summary>
        char ScreenCharacter { get; }
    }
}
