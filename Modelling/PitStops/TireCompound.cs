using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Modelling.PitStops.Interfaces;

namespace WhatIfF1.Modelling.PitStops
{
    public class TireCompound : ITireCompound
    {
        public string ScreenName { get; }
        public Color ScreenColor { get; }
        public char ScreenCharacter { get; }
        public string ImagePath { get; }

        public TireCompound(string screenName, Color screenColor)
        {
            ScreenName = screenName;
            ScreenColor = screenColor;
            ScreenCharacter = screenName[0];

            string tireFolder = FileAdapter.Instance.TiresRoot;
            ImagePath = Path.Combine(tireFolder, $"{screenName}.png");
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
