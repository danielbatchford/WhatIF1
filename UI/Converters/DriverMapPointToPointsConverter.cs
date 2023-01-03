using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using WhatIfF1.UI.Controller.TrackMaps;

namespace WhatIfF1.UI.Converters
{
    public class DriverMapPointToPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new PointCollection(((ObservableCollection<DriverMapPoint>)value).Select(driverMapPoint => driverMapPoint.Point));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed, only one way conversion neccessary
            return null;
        }
    }
}
