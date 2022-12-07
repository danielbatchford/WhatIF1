using System.Windows.Media;

namespace WhatIfF1.Modelling.Tires
{
    public static class TireCompoundStore
    {
        public static TireCompound SoftTyre { get; } = new TireCompound("Soft", Color.FromRgb(221, 46, 48));
        public static TireCompound MediumTyre { get; } = new TireCompound("Medium", Color.FromRgb(253, 203, 88));
        public static TireCompound HardTyre { get; } = new TireCompound("Hard", Color.FromRgb(230, 231, 232));
    }
}
