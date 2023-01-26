using System;
using System.Globalization;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public class SecToMsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble((int)value);
            return val * 1000;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble((int)value);
            return val / 1000;
        }
    }
}