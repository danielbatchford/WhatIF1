using System.Collections.ObjectModel;
using WhatIfF1.Modelling.Events.TrackEvents;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
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
                OnSelectedMarkerChanged();
                OnPropertyChanged();
            }
        }

        private readonly IEventController _parentController;

        public MarkerProvider(IEventController parentController)
        {
            _parentController = parentController;

            _markerStore = TrackMarkerStore.Instance;

            Markers = new ObservableCollection<ITrackMarker>
            {
                _markerStore.CreateRaceStartMarker(),
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

        private void OnSelectedMarkerChanged()
        {
            if (SelectedMarker == null)
            {
                return;
            }

            _parentController.CurrentTime = SelectedMarker.StartMs;
        }
    }
}
