
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = System.Windows.Media.Color;

namespace WhatIfF1.Util.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Sample n random points around the image. Count distinct colours and sorts them by frequency
        /// </summary>
        public static IEnumerable<Color> GetDominantColors(this Image image)
        {
            int nSamples = 20;
            Random random = new Random();

            Bitmap bitmap = (Bitmap)image;

            IDictionary<Color, int> sampleColors = new Dictionary<Color, int>();

            for (int i = 0; i < nSamples; i++)
            {
                int x = random.Next(0, bitmap.Width);
                int y = random.Next(0, bitmap.Height);

                System.Drawing.Color drawColor = bitmap.GetPixel(x, y);
                Color mediaColor = Color.FromRgb(drawColor.R, drawColor.G, drawColor.B);

                if (sampleColors.ContainsKey(mediaColor))
                {
                    sampleColors[mediaColor]++;
                }
                else
                {
                    sampleColors.Add(mediaColor, 1);
                }
            }

            return from item in sampleColors orderby item.Value ascending select item.Key;
        }
    }
}
