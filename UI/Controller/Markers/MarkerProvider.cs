using System;
using System.Collections.Generic;
using System.Linq;
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
            var raceStartMarker = _markerStore.CreateRaceStartMarker();

            var trackStates = _parentController.DataProvider.GetTrackStates();
            var trackStateMarkers = CreateTrackStateMarkers(trackStates);

            return new List<ITrackMarker>(trackStateMarkers)
            {
                raceWinMarker,
                raceStartMarker
            };
        }

        private IEnumerable<ITrackMarker> CreateTrackStateMarkers(IEnumerable<ITrackState> trackStates)
        {
            var markers = new List<ITrackMarker>(trackStates.Count());

            foreach (ITrackState trackState in trackStates)
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