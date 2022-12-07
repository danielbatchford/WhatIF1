using System;
using System.Globalization;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public sealed class ToFormattedTimeConverter : IValueConverter
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
            string ToNdpString(int nDigits, int val)
            {
                string msString = val.ToString();
                int msLength = msString.Length;

                if (msLength == nDigits)
                {
                    return msString;
                }

                string zeroPadding = "0";

                for (int i = 0; i < nDigits - msLength - 1; i++)
                {
                    zeroPadding += "0";
                }

                return zeroPadding + msString;
            }

            int milliSeconds = (int)value;

            TimeSpan time = TimeSpan.FromMilliseconds(milliSeconds);
            char sign = milliSeconds >= 0 ? '+' : '-';

            if(time.TotalHours >= 1)
            {
                return $"{sign}{time.Hours}:{ToNdpString(2, time.Minutes)}:{ToNdpString(2, time.Seconds)}:{ToNdpString(3, time.Milliseconds)}";
            }
            else if (time.TotalMinutes >= 1)
            {
                return $"{sign}{ToNdpString(2, time.Minutes)}:{ToNdpString(2, time.Seconds)}:{ToNdpString(3, time.Milliseconds)}";
            }
            else
            {
                return $"{sign}{time.Seconds}:{ToNdpString(3, time.Milliseconds)}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
