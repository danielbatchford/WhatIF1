using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.UI.Controller.Graphing;
using WhatIfF1.UI.Controller.TrackMaps;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller
{
    public class EventController : NotifyPropertyChangedWrapper, IDisposable
    {
        private readonly DispatcherTimer _timer;

        private readonly PlaybackParameterContainer _playbackParams;

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

        public GraphProvider VelocityLapGraphProvider { get; }

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

                OnCurrentTimeChanged();
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
                OnCurrentLapChanged();
                OnPropertyChanged();
            }
        }

        private DriverStanding _selectedStanding;

        public DriverStanding SelectedStanding
        {
            get => _selectedStanding;
            set 
            {
                if(_selectedStanding != null && _selectedStanding.Equals(value))
                {
                    return;
                }

                _selectedStanding = value;
                OnSelectedStandingChanged();
                OnPropertyChanged();
            }
        }

        private bool _playing;

        public bool Playing
        {
            get => _playing;
            set 
            {
                if(_playing == value)
                {
                    return;
                }

                _playing = value;
                OnPlayingChanged(value);
                OnPropertyChanged();
            }
        }

        private ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                if(_playPauseCommand is null)
                {
                    _playPauseCommand = new CommandHandler(() => Playing = !Playing, () => true);
                }

                return _playPauseCommand;
            }
            set
            {
                _playPauseCommand = value;
                OnPropertyChanged();
            }
        }

        public EventController(Track track, EventModel model)
        {
            Model = model;

            MapProvider = new TrackMapProvider(track, model.GetDrivers());
            VelocityLapGraphProvider = new GraphProvider(this, GraphType.VELOCITY_TIME);

            CurrentTime = 0;

            var standings = model.GetStandingsAtTime(CurrentTime, out int currentLap);
            CurrentLap = currentLap;

            Standings = new ObservableRangeCollection<DriverStanding>(standings);

            _playbackParams = PlaybackParameterContainer.GetDefault();

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(_playbackParams.TimerUpdateMsIncrement), DispatcherPriority.ApplicationIdle, TimerTick, Dispatcher.CurrentDispatcher);
            // Don't start timer by default
            _timer.Stop();
        }

        public void Dispose()
        {
            Playing = false;
        }

        private void OnCurrentTimeChanged()
        {
            var newStandings = Model.GetStandingsAtTime(CurrentTime, out int currentLap);

            // Update current lap
            CurrentLap = currentLap;

            if (!Standings.SequenceEqual(newStandings))
            {
                if(Standings.Count() != newStandings.Count())
                {
                    MapProvider.UpdateRetirements(newStandings, Standings);
                }

                Standings.ReplaceRange(newStandings);

                // Update the selected standing to the lead driver standing if this driver standing is no longer in the race (e.g has retired)
                if (SelectedStanding != null && !Standings.Contains(SelectedStanding))
                {
                    SelectedStanding = Standings.First();
                }
            }

            // Update driver positions on the map
            foreach (DriverStanding standing in Standings)
            {
                MapProvider.UpdateDriverMapPosition(standing.Driver, standing.ProportionOfLap);
            }
        }

        private void OnCurrentLapChanged()
        {
            VelocityLapGraphProvider.UpdateGraph();
        }

        private void OnSelectedStandingChanged()
        {
            if (SelectedStanding != null)
            {
                VelocityLapGraphProvider.UpdateCurrentDriver(SelectedStanding.Driver);
            }
            else
            {
                VelocityLapGraphProvider.RemoveCurrentDriver();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            CurrentTime += _playbackParams.MsIncrement;
        }

        private void OnPlayingChanged(bool playing)
        {
            if (_timer.IsEnabled && !playing)
            {
                _timer.Stop();
                Logger.Instance.Info("Playback stopped");
                return;
            }

            if (!_timer.IsEnabled && playing)
            {
                _timer.Start();
                Logger.Instance.Info("Playback started");
                return;
            }

            throw new ScenarioException("Mismatch between event controller playback states occured");
        }
    }
}
