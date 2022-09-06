using System;
using System.Globalization;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public class ToFormattedTimeConverter : IValueConverter
    {
        /// <summary>
        /// Formats millisecond times into the standard F1 format.
        /// 100ms -> +0.100, -50ms -> -0.050, 601051ms -> +1:01:51 etc
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ToNdpString(int n, int ms)
            {
                string msString = ms.ToString();
                int msLength = msString.Length;

                if (msLength == n)
                {
                    return msString;
                }

                string zeroPadding = "0";

                for (int i = 0; i < n - msLength - 1; i++)
                {
                    zeroPadding += "0";
                }

                return zeroPadding + msString;
            }

            int milliSeconds = (int)value;

            DateTime time = new DateTime(10000 * Math.Abs(milliSeconds));
            char sign = milliSeconds >= 0 ? '+' : '-';

            if(time.Minute >= 1)
            {
                return $"{sign}{time.Minute}:{time.Second}:{ToNdpString(3, time.Millisecond)}";
            }
            else
            {
                return $"{sign}{time.Second}:{ToNdpString(3, time.Millisecond)}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
