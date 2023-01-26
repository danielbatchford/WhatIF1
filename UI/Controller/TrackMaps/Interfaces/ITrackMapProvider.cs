using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.TrackMaps.Interfaces
{
    public interface ITrackMapProvider
    {
        PointCollection TrackPoints { get; }
        ObservableCollection<IDriverMapPoint> DriverPoints { get; }
        Point StartPoint { get; }

        void UpdateDriverMapPosition(IDriverStanding standing);
        void ToSelectedDriverMode(IDriver driver);
        void ClearSelectedDriverMode();
    }
}
