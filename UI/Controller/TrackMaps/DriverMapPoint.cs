using System.Windows;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public class DriverMapPoint : NotifyPropertyChangedWrapper
    {
        private const double _retiredOpacity = 0.6;

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

        private bool _isRetired;
        public bool IsRetired
        {
            get => _isRetired;
            set
            {
                if (_isRetired == value)
                {
                    return;
                }

                _isRetired = value;

                // Update opacity based on the state of retirement
                if (value)
                {
                    Opacity = _retiredOpacity;
                }
                else
                {
                    Opacity = 1;
                }

                OnPropertyChanged();
            }
        }

        private double _opacity;

        public double Opacity
        {
            get => _opacity;
            set 
            {
                if(_opacity == value)
                {
                    return;
                }

                _opacity = value;
                OnPropertyChanged();
            }
        }


        public DriverMapPoint(Driver driver, Point point)
        {
            Driver = driver;
            Point = point;
            Color = driver.Constructor.Color;
            Opacity = 1;

            IsRetired = false;
        }
    }
}
