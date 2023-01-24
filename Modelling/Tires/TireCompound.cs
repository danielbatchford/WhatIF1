using System.Collections.Generic;
using System.Windows.Media;
using WhatIfF1.Modelling.Tires.Interfaces;

namespace WhatIfF1.Modelling.Tires
{
    public class TireCompound : ITireCompound
    {
        public string ScreenName { get; }
        public Color ScreenColor { get; }

        public char ScreenCharacter { get; }

        public TireCompound(string screenName, Color screenColor)
        {
            ScreenName = screenName;
            ScreenColor = screenColor;

            ScreenCharacter = screenName[0];
        }

        public override string ToString()
        {
            return ScreenName;
        }

        public override int GetHashCode()
        {
            int hashCode = 902971198;
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ScreenName);
            hashCode *= -1521134295 + ScreenCharacter.GetHashCode();
            return hashCode;
        }

        public bool Equals(ITireCompound other)
        {
            return other is TireCompound compound && compound.ScreenName.Equals(ScreenName);
        }
    }
}
