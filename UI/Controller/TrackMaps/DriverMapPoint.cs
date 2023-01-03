using System.Windows;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public class DriverMapPoint : NotifyPropertyChangedWrapper
    {
        public Driver Driver { get; }

        public Color Color { get; }

        private Point _point;

        public Point Point
        {
            get => _point;
            set 
            {
                if (_point.Equals(value))
                {
                    return;
                }
                _point = value;
                OnPropertyChanged();
            }
        }


        public DriverMapPoint(Driver driver, Point point)
        {
            Driver = driver;
            Point = point;
            Color = driver.Constructor.Color;
        }
    }
}
