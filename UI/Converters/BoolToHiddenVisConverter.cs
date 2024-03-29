﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WhatIfF1.UI.Converters
{
    public sealed class BoolToHiddenVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
