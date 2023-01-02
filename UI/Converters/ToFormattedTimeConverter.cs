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

            int milliseconds = (int)value;

            string sign = GetSign(milliseconds);

            TimeSpan absTime = TimeSpan.FromMilliseconds(Math.Abs(milliseconds));

            string hours = absTime.Hours.ToString();
            string mins = absTime.Minutes.ToString();
            string secs = ToNdpString(2, absTime.Seconds);
            string ms = ToNdpString(3, absTime.Milliseconds);

            if (Math.Abs(absTime.TotalHours) >= 1)
            {
                return $"{sign}{hours}:{mins}:{secs}:{ms}";
            }
            else if (Math.Abs(absTime.TotalMinutes) >= 1)
            {
                return $"{sign}{mins}:{secs}:{ms}";
            }
            else
            {
                return $"{sign}{secs}:{ms}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetSign(double ms)
        {
            if (ms == 0)
            {
                return string.Empty;
            }

            return ms > 0 ? "+" : "-";
        }
    }
}
