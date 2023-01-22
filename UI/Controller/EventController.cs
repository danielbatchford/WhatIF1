using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.Modelling.Tracks.Interfaces;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.UI.Controller.DataBuffering;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Graphing;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.UI.Controller.TrackMaps;
using WhatIfF1.UI.Controller.TrackMaps.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller
{
    public class EventController : NotifyPropertyChangedWrapper, IEventController
    {
        private readonly DispatcherTimer _timer;

        private readonly IPlaybackParameterContainer _playbackParams;

        public IEventModelDataProvider DataProvider { get; }

        private int _currentTime;

        public ObservableCollection<IDriverStanding> Standings { get; }

        public ITrackMapProvider MapProvider { get; }

        public IGraphProvider GraphProvider { get; }

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
                if (value > DataProvider.Model.TotalTime)
                {
                    throw new EventControllerException($"Requested current time exceeds the maximum model time (Requested {value}, Max time is {DataProvider.Model.TotalTime}");
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
            set
            {
                if (value > DataProvider.Model.NoOfLaps)
                {
                    throw new EventControllerException($"Attempted to set max lap to {value} while only {DataProvider.Model.NoOfLaps} existed");
                }

                _currentLap = value;
                OnCurrentLapChanged();
                OnPropertyChanged();
            }
        }

        private IDriverStanding _selectedStanding;

        public IDriverStanding SelectedStanding
        {
            get => _selectedStanding;
            set
            {
                if (_selectedStanding?.Equals(value) == true)
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
                if (_playing == value)
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
                return _playPauseCommand ?? (_playPauseCommand = new CommandHandler(() => Playing = !Playing, () => true));
            }
            set
            {
                _playPauseCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _deselectStandingCommand;
        public ICommand DeselectStandingCommand
        {
            get
            {
                return _deselectStandingCommand ?? (_deselectStandingCommand = new CommandHandler(() => SelectedStanding = null, () => SelectedStanding != null));
            }
            set
            {
                _deselectStandingCommand = value;
                OnPropertyChanged();
            }
        }

        public EventController(ITrack track, IEventModel model)
        {
            _playbackParams = PlaybackParameterContainer.GetDefault();

            DataProvider = new EventModelDataProvider(model, _playbackParams);
            MapProvider = new TrackMapProvider(track, model.GetDrivers());
            GraphProvider = new GraphProvider(this, GraphType.VELOCITY_TIME);

            CurrentTime = 0;

            var standings = model.GetStandingsAtTime(CurrentTime, out int currentLap);
            CurrentLap = currentLap;

            Standings = new ObservableCollection<IDriverStanding>(standings);


            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(_playbackParams.TimerUpdateMsIncrement), DispatcherPriority.ApplicationIdle, TimerTick, Dispatcher.CurrentDispatcher);
            // Don't start timer by default
            _timer.Stop();
        }

        public void Dispose()
        {
            Playing = false;
        }

        private async void OnCurrentTimeChanged()
        {
            IEventModelDataPacket frame = await DataProvider.GetDataAtTime(CurrentTime);

            // Update current lap
            CurrentLap = frame.CurrentLap;

            if (!Standings.SequenceEqual(frame.Standings))
            {
                foreach(var standing in frame.Standings)
                {
                    var standingToUpdate = Standings.Single(s => s.Driver.Equals(standing.Driver));
                    standingToUpdate.UpdateFromOtherStanding(standing);
                }

                MapProvider.UpdateNotRunning(Standings);
            }

            // Update driver positions on the map
            foreach (DriverStanding standing in Standings)
            {
                MapProvider.UpdateDriverMapPosition(standing);
            }
        }

        private void OnCurrentLapChanged()
        {
            GraphProvider.UpdateGraph();
        }

        private void OnSelectedStandingChanged()
        {
            if (SelectedStanding != null)
            {
                MapProvider.ToSelectedDriverMode(SelectedStanding.Driver);
                GraphProvider.UpdateCurrentDriver(SelectedStanding.Driver);
            }
            else
            {
                MapProvider.ClearSelectedDriverMode();
                GraphProvider.RemoveCurrentDriver();
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
