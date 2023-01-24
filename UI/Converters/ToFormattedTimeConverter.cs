using System;
using System.Globalization;
using System.Windows.Data;
using WhatIfF1.Util.Extensions;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StringExtensions.ToF1TimingScreenFormat((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
