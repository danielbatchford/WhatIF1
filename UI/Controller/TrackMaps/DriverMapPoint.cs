using System.Windows;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.TrackMaps.Interfaces;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public class DriverMapPoint : NotifyPropertyChangedWrapper, IDriverMapPoint
    {
        public IDriver Driver { get; }

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

        private bool _isNotRunning;
        public bool IsNotRunning
        {
            get => _isNotRunning;
            set
            {
                if (_isNotRunning == value)
                {
                    return;
                }

                _isNotRunning = value;
                OnPropertyChanged();
            }
        }

        private double _opacity;

        public double Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity == value)
                {
                    return;
                }
                _opacity = value;
                OnPropertyChanged();
            }
        }

        public DriverMapPoint(IDriver driver, Point point)
        {
            Driver = driver;
            Point = point;
            Color = driver.Constructor.Color;
            Opacity = 1;
        }
    }
}
