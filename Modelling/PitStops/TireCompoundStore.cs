using System;
using System.Windows.Media;
using WhatIfF1.Modelling.PitStops.Interfaces;

namespace WhatIfF1.Modelling.PitStops
{
    public static class TireCompoundStore
    {
        public static ITireCompound SoftTyre { get; } = new TireCompound("Soft", Color.FromRgb(221, 46, 48));
        public static ITireCompound MediumTyre { get; } = new TireCompound("Medium", Color.FromRgb(253, 203, 88));
        public static ITireCompound HardTyre { get; } = new TireCompound("Hard", Color.FromRgb(230, 231, 232));
        public static ITireCompound IntermediateTire { get; } = new TireCompound("Intermediate", Color.FromRgb(54, 182, 73));
        public static ITireCompound WetTire { get; } = new TireCompound("Wet", Color.FromRgb(0, 174, 239));

        // TODO - remove
        private static readonly Random _random = new Random();
        public static ITireCompound GetRandomCompound()
        {
            int i = _random.Next(0, 5);

            ITireCompound compound;
            switch (i)
            {
                case 0:
                    compound = SoftTyre;
                    break;

                case 1:
                    compound = MediumTyre;
                    break;

                case 2:
                    compound = HardTyre;
                    break;

                case 3:
                    compound = IntermediateTire;
                    break;

                case 4:
                    compound = WetTire;
                    break;

                default:
                    throw new Exception("Should not occur");
            }

            return compound;
        }
    }
}
