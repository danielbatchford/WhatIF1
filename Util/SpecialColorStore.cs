using System;
using System.Windows.Media;

namespace WhatIfF1.Util
{
    public class SpecialColorStore
    {

        #region LazyInitialization

        public static SpecialColorStore Instance => _lazy.Value;

        private readonly static Lazy<SpecialColorStore> _lazy = new Lazy<SpecialColorStore>(() => new SpecialColorStore());

        #endregion LazyInitialization

        public Color GreenColor { get; }
        public Color YellowColor { get; }
        public Color RedColor { get; }

        private SpecialColorStore()
        {
            RedColor = Color.FromRgb(238, 33, 9);
            YellowColor = Color.FromRgb(240, 193, 9);
            GreenColor = Color.FromRgb(0, 208, 6);
        }
    }
}
