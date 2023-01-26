using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events.TrackEventMarking;
using WhatIfF1.Modelling.Events.TrackEvents;
using WhatIfF1.Modelling.Events.TrackEvents.Interfaces;
using WhatIfF1.Modelling.TrackStates;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.UI.Controller.Markers.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Markers
{
    public class MarkerProvider : LoadableBindableBase, IMarkerProvider
    {
        public SortedObservableRangeCollection<ITrackMarker> Markers { get; }

        private readonly TrackMarkerFactory _markerStore;

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
            _markerStore = TrackMarkerFactory.Instance;

            Markers = new SortedObservableRangeCollection<ITrackMarker>(new MarkerComparer());

            // Begin loading markers

            IsLoading = true;
            try
            {
                var staticTask = Task
                    .Run(() => CreateStaticMarkers())
                    .ContinueWith(task => Markers.AddRange(task.Result),
                    TaskScheduler.FromCurrentSynchronizationContext());

                var nonStaticTask = Task
                    .Run(() => CreateDriverMarkers())
                    .ContinueWith(task => Markers.AddRange(task.Result),
                    TaskScheduler.FromCurrentSynchronizationContext());

                Task.WhenAll(staticTask, nonStaticTask).ContinueWith(_ =>
                {
                    IsLoading = false;
                    IsLoaded = Markers.Count > 0;
                    Logger.Instance.Info("Track markers successfully created");
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception e)
            {
                IsLoaded = false;
                IsLoading = false;
                Logger.Instance.Exception(e);
            }
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

        private async Task<IEnumerable<ITrackMarker>> CreateStaticMarkers()
        {
            int lastMs = _parentController.TotalTime - 1;
            var lastPacket = await _parentController.DataProvider.GetDataAtTime(lastMs);

            var standings = lastPacket.Standings;
            var lastLap = lastPacket.CurrentLap;

            var markers = new List<ITrackMarker>
            {
                _markerStore.CreateRaceWinMarker(standings[0].Driver, lastMs, lastLap),
                _markerStore.CreateRaceStartMarker()
            };

            foreach (ITrackState trackState in _parentController.DataProvider.GetTrackStates())
            {
                ITrackMarker marker = null;

                if (trackState.Flag == FlagType.GREEN)
                {
                    marker = _markerStore.CreateGreenFlagMarker(trackState.StartMs, trackState.StartLap);
                }
                else if (trackState.Flag == FlagType.RED)
                {
                    marker = _markerStore.CreateRedFlagMarker(trackState.StartMs, trackState.StartLap);
                }
                else if (trackState.Flag == FlagType.YELLOW)
                {
                    switch (trackState.SafetyCarState)
                    {
                        case SafetyCarState.SC:
                            marker = _markerStore.CreateSafetyCarMarker(trackState.StartMs, trackState.EndMs, trackState.StartLap, trackState.EndLap);
                            break;

                        case SafetyCarState.VSC:
                            marker = _markerStore.CreateVirtualSafetyCarMarker(trackState.StartMs, trackState.EndMs, trackState.StartLap, trackState.EndLap);
                            break;

                        case SafetyCarState.NONE:
                            marker = _markerStore.CreateYellowFlagMarker(trackState.StartMs, trackState.EndMs, trackState.StartLap, trackState.EndLap);
                            break;
                    }
                }
                else
                {
                    throw new Exception($"Cannot create track state markers: Unrecognised flag type: {trackState.Flag}");
                }

                markers.Add(marker);
            }

            return markers;
        }

        private async Task<IEnumerable<ITrackMarker>> CreateDriverMarkers()
        {
            // Overtake markers
            // retirement markers
            // pit stop markers
            //TODO

            return new List<ITrackMarker>();
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