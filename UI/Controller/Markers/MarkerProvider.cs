using System.Collections.ObjectModel;
using System.Linq;
using WhatIfF1.Modelling.Events.TrackEvents;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Markers.Interfaces;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.Markers
{
    public class MarkerProvider : NotifyPropertyChangedWrapper, IMarkerProvider
    {
        public ObservableCollection<ITrackMarker> Markers { get; }

        private readonly TrackMarkerStore _markerStore;

        private ITrackMarker _selectedMarker;
        public ITrackMarker SelectedMarker
        {
            get => _selectedMarker;
            set
            {
                if (_selectedMarker?.Equals(value) == true)
                {
                    return;
                }
                _selectedMarker = value;
                OnPropertyChanged();
            }
        }

        private readonly IEventModelDataProvider _dataProvider;

        public MarkerProvider(IEventModelDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

            _markerStore = TrackMarkerStore.Instance;

            var driver = _dataProvider.Model.GetDrivers().Single(d => d.DriverLetters == "RIC");

            Markers = new ObservableCollection<ITrackMarker>
            {
                _markerStore.CreatePitStopMarker(driver, 0, 100, 1),
                _markerStore.CreateRedFlagMarker(224, 4),
                _markerStore.CreateYellowFlagMarker(1234, 1245, 1, 4),
                _markerStore.CreateRetirementMarker(driver, 234, 4),
                _markerStore.CreateSafetyCarMarker(124, 645, 1, 5),
                _markerStore.CreateOvertakeMarker(driver, 100, 2),
                _markerStore.CreateVirtualSafetyCarMarker(123, 54345, 3, 43),
                _markerStore.CreateRaceWinMarker(driver, 20, 2),
                _markerStore.CreateRaceStartMarker(),
                _markerStore.CreateGreenFlagMarker(234, 4)
            };
        }

        public void BeginMarkerLoading()
        {
        }

        public void PauseMarkerLoading()
        {
        }

        public void InvalidateMarkers(int startMs, int endMs)
        {
        }
    }
}
