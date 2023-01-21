using System;

namespace WhatIfF1.Util.Extensions
{
    public static class StringExtensions
    {
        public static void OpenInBrowser(this string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public static string ToF1TimingScreenFormat(int milliseconds)
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

            string GetSign(double msVal)
            {
                if (msVal == 0)
                {
                    return string.Empty;
                }

                return msVal > 0 ? "+" : "-";
            }

            string sign = GetSign(milliseconds);

            TimeSpan absTime = TimeSpan.FromMilliseconds(Math.Abs(milliseconds));

            string hours = absTime.Hours.ToString();
            string secs = ToNdpString(2, absTime.Seconds);
            string ms = ToNdpString(3, absTime.Milliseconds);

            if (Math.Abs(absTime.TotalHours) >= 1)
            {
                return $"{sign}{hours}:{ToNdpString(2, absTime.Minutes)}:{secs}:{ms}";
            }
            else if (Math.Abs(absTime.TotalMinutes) >= 1)
            {
                return $"{sign}{absTime.Minutes}:{secs}:{ms}";
            }
            else
            {
                return $"{sign}{secs}:{ms}";
            }
        }
    }
}
