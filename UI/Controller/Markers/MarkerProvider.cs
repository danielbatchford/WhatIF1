using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events.TrackEventMarking;
using WhatIfF1.Modelling.Events.TrackEvents;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.UI.Controller.Markers.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Markers
{
    public class MarkerProvider : LoadableBindableBase, IMarkerProvider
    {
        public SortedObservableRangeCollection<ITrackMarker> Markers { get; }

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

            Markers = new SortedObservableRangeCollection<ITrackMarker>(new MarkerComparer());

            // Begin loading markers

            IsLoading = true;
            try
            {
                Task.Run(() => CreateMarkers()).ContinueWith(task =>
                {
                    Markers.AddRange(task.Result);
                    IsLoaded = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception e)
            {
                Logger.Instance.Exception(e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<IEnumerable<ITrackMarker>> CreateMarkers()
        {
            int lastMs = _parentController.TotalTime - 1;
            var lastPacket = await _parentController.DataProvider.GetDataAtTime(lastMs);

            var standings = lastPacket.Standings;
            var lastLap = lastPacket.CurrentLap;

            var raceWinMarker = _markerStore.CreateRaceWinMarker(standings[0].Driver, lastMs, lastLap);

            return new List<ITrackMarker>
            {
                raceWinMarker,
                _markerStore.CreateRaceStartMarker(),
            };
        }

        public void ClearMarkers(int startMs, int endMs)
        {
            var toRemove = new List<ITrackMarker>();

            foreach (var marker in Markers)
            {
                if (marker.StartMs < startMs)
                {
                    continue;
                }

                if (marker.EndMs > endMs)
                {
                    break;
                }

                toRemove.Add(marker);
            }

            if (toRemove.Count > 0)
            {
                Markers.RemoveRange(toRemove);
            }
        }

        public void ClearMarkers()
        {
            Markers.Clear();
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
