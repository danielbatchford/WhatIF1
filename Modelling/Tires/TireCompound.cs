using System.Collections.Generic;
using System.Windows.Media;

namespace WhatIfF1.Modelling.Tires
{
    public class TireCompound
    {
        public string ScreenName { get; }
        public Color ScreenColor { get; }

        /// <summary>
        /// e.g S, M or H
        /// </summary>
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

        public override bool Equals(object other)
        {
            return other is TireCompound compound && compound.ScreenName.Equals(ScreenName);
        }

        public override int GetHashCode()
        {
            int hashCode = 902971198;
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ScreenName);
            hashCode *= -1521134295 + ScreenCharacter.GetHashCode();
            return hashCode;
        }
    }
}
