using System;
using System.Globalization;

namespace WhatIfF1.Util.Extensions
{
    public static class DateTimeExtensions
    {
        private const string _iso8601LongFormat = "yyyy-MM-ddTHH:mm:ss.fff";

        // For optimisation
        private static readonly int _formatLength = _iso8601LongFormat.Length;

        public static DateTime FromLongIso8601String(string isoString)
        {
            // Remove trailing z
            isoString = isoString.Substring(0, _formatLength);
            return DateTime.ParseExact(isoString, _iso8601LongFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
