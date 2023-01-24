using System;
using System.Globalization;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public class MsToHundredSecConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return i * 100000;
            }
            else if (value is double d)
            {
                return d * 100000;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return ((double)i) / 100000;
            }
            else if (value is double d)
            {
                return d / 100000;
            }

            return null;
        }
    }
}