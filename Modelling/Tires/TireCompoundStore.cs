using System.Windows.Media;
using WhatIfF1.Modelling.Tires.Interfaces;

namespace WhatIfF1.Modelling.Tires
{
    public static class TireCompoundStore
    {
        public static ITireCompound SoftTyre { get; } = new TireCompound("Soft", Color.FromRgb(221, 46, 48));
        public static ITireCompound MediumTyre { get; } = new TireCompound("Medium", Color.FromRgb(253, 203, 88));
        public static ITireCompound HardTyre { get; } = new TireCompound("Hard", Color.FromRgb(230, 231, 232));
    }
}
