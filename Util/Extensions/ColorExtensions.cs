using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WhatIfF1.Util.Extensions
{
    public static class ColorExtensions
    {
        private readonly static Random _random = new Random();

        public static Color GetRandomColor()
        {
            return Color.FromRgb
                (
                    (byte)_random.Next(0, 255),
                    (byte)_random.Next(0, 255),
                    (byte)_random.Next(0, 255)
                );
        }
    }
}
