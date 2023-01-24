using System.Windows;
using System.Windows.Media;

namespace WhatIfF1.UI.Controller.TrackMaps.Interfaces
{
    public interface IMapPoint
    {
        Point Point { get; set; }

        Color Color { get; }

        double Opacity { get; set; }
    }
}
