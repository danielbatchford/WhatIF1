using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public sealed class IsNotNullToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value).Equals(Visibility.Visible);
        }
    }
}