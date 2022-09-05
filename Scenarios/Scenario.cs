using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Scenarios
{
    public class Scenario : PropertyChangedWrapper, ICloneable
    {
        public string EventName { get; }
        public Track Track { get; }
        public DateTime EventDate { get; }

        public string WikipediaLink { get; }

        public int Round { get; }

        private Color _color;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get => _isLoaded;
            private set
            {
                _isLoaded = value;
            }
        }

        private ICommand _loadRaceCommand;

        public ICommand LoadRaceCommand
        {
            get
            {
                if(_loadRaceCommand is null)
                {
                    bool canExecute = !IsLoading && !IsLoaded;
                    _loadRaceCommand = new CommandHandler(() => LoadRaceFromAPI(), () => canExecute);
                }

                return _loadRaceCommand;
            }
                set 
            {
                _loadRaceCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _aboutRaceCommand;

        public ICommand AboutRaceCommand
        {
            get
            {
                if (_aboutRaceCommand is null)
                {
                    bool canExecute = !string.IsNullOrEmpty(WikipediaLink);
                    _aboutRaceCommand = new CommandHandler(() => WikipediaLink.OpenInBrowser(), () => canExecute);
                }

                return _aboutRaceCommand;
            }
            set 
            {
                _aboutRaceCommand = value;
                OnPropertyChanged();
            }
        }



        public Scenario(JObject eventJson)
        {
            Round = eventJson["round"].Value<int>();
            WikipediaLink = eventJson["url"].Value<string>();
            EventName = eventJson["raceName"].Value<string>();

            string eventDate = eventJson["date"].Value<string>();
            string eventTime = eventJson["time"].Value<string>();

            EventDate = DateTime.ParseExact($"{eventDate}-{eventTime}", "yyyy-MM-dd-HH:mm:ssZ", CultureInfo.InvariantCulture);

            // Build track object from inner Json
            Track = new Track(eventJson["Circuit"].ToObject<JObject>());

            Color = ColorExtensions.GetRandomColor();
        }

        private void LoadRaceFromAPI()
        {
            Logger.Instance.Info($"Loading race data for the {EventName}");
            IsLoading = true;

            try
            {
                Logger.Instance.Info($"Loaded race data for the {EventName}");
                IsLoaded = true;
            }
            catch (ScenarioLoadException e)
            {
                Logger.Instance.Exception(e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
