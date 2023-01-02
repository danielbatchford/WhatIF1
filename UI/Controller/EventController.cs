using System.Linq;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.UI.Controller.TrackMaps;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller
{
    public class EventController : PropertyChangedWrapper
    {
        private EventModel _model;

        public EventModel Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        private int _currentTime;

        public ObservableRangeCollection<DriverStanding> Standings { get; }

        public TrackMapProvider MapProvider { get; }

        public int CurrentTime
        {
            get => _currentTime;
            set
            {
                if (value < 0)
                {
                    throw new EventControllerException($"Requested time was less than 0. (Got {value})");
                }

                // No need to update if the value equals the current time
                if (value == _currentTime)
                {
                    return;
                }

                // If the current time exceeds the maximum time in the model, throw an exception
                if (value > Model.TotalTime)
                {
                    throw new EventControllerException($"Requested current time exceeds the maximum model time (Requested {value}, Max time is {Model.TotalTime}");
                }

                _currentTime = value;

                UpdateAtTime();
                OnPropertyChanged();
            }
        }

        private int _currentLap;

        public int CurrentLap
        {
            get => _currentLap;
            private set
            {
                if (value > Model.NoOfLaps)
                {
                    throw new EventControllerException($"Attempted to set max lap to {value} while only {Model.NoOfLaps} existed");
                }

                _currentLap = value;
                OnPropertyChanged();
            }
        }

        public EventController(Track track, EventModel model)
        {
            Model = model;

            MapProvider = new TrackMapProvider(track, model.GetDrivers());

            CurrentTime = 0;

            var standings = model.GetStandingsAtTime(CurrentTime, out int currentLap);
            CurrentLap = currentLap;

            Standings = new ObservableRangeCollection<DriverStanding>(standings);
        }

        private void UpdateAtTime()
        {
            var newStandings = Model.GetStandingsAtTime(CurrentTime, out int currentLap);

            // Update current lap
            CurrentLap = currentLap;

            if (!Standings.SequenceEqual(newStandings))
            {
                Standings.ReplaceRange(newStandings);
            }

            // Update driver positions on the map
            foreach (DriverStanding standing in Standings)
            {
                MapProvider.UpdateDriverMapPosition(standing.Driver, standing.ProportionOfLap);
            }

        }
    }
}
