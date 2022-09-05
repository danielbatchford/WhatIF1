using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Scenarios
{
    public class Scenario : PropertyChangedWrapper, ICloneable, IEquatable<Scenario>
    {
        /// <summary>
        /// Used for equality checks
        /// </summary>
        public Guid Id { get; }
        public string EventName { get; }
        public Track Track { get; }
        public DateTime EventDate { get; }
        public string WikipediaLink { get; }

        public int Round { get; }

        private int _numLaps;

        public int NumLaps
        {
            get => _numLaps;
            set 
            {
                if(value <= 0)
                {
                    throw new ScenarioLoadException($"Invalid number of laps provided. Got {value} laps");
                }

                _numLaps = value;
                OnPropertyChanged();
            }
        }


        private Color _primaryColor;

        public Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                OnPropertyChanged();
            }
        }

        private Color _secondaryColor;

        public Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                _secondaryColor = value;
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

        private ICommand _removeScenarioCommand;

        public ICommand RemoveScenarioCommand
        {
            get
            {
                if (_removeScenarioCommand is null)
                {
                    _removeScenarioCommand = new CommandHandler(() => 
                    {
                        ScenarioStore.Instance.RemoveScenario(this);
                    }, () => true);
                }

                return _removeScenarioCommand;
            }
            set
            {
                _removeScenarioCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _cloneScenarioCommand;

        public ICommand CloneScenarioCommand
        {
            get
            {
                if(_cloneScenarioCommand is null)
                {
                    _cloneScenarioCommand = new CommandHandler(() =>
                    {
                        ScenarioStore.Instance.CloneScenario(this);
                    }, () => true);
                }

                return _cloneScenarioCommand;
            }
            set
            {
                _removeScenarioCommand = value;
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

        public EventModel Model { get; private set; }

        public Scenario(JObject eventJson)
        {
            Id = Guid.NewGuid();

            Round = eventJson["round"].Value<int>();
            WikipediaLink = eventJson["url"].Value<string>();
            EventName = eventJson["raceName"].Value<string>();

            string eventDate = eventJson["date"].Value<string>();
            string eventTime = eventJson["time"].Value<string>();

            EventDate = DateTime.ParseExact($"{eventDate}-{eventTime}", "yyyy-MM-dd-HH:mm:ssZ", CultureInfo.InvariantCulture);

            // Build track object from inner Json
            Track = new Track(eventJson["Circuit"].ToObject<JObject>());

            // Extract color from flag color
            using(System.Drawing.Image flag = System.Drawing.Image.FromFile(Track.FlagFilePath))
            {
                IList<Color> sortedColors = flag.GetDominantColors().ToList();

                PrimaryColor = sortedColors[0];

                // Case where flag only has one color
                if (sortedColors.Count == 1)
                {
                    SecondaryColor = PrimaryColor;
                }
                else
                {
                    SecondaryColor = sortedColors[1];
                }
            }
        }

        private async void LoadRaceFromAPI()
        {
            Logger.Instance.Info($"Loading race data for the {EventName}");
            IsLoading = true;

            try
            {
                int year = EventDate.Year;

                // Fetch driver data for the event
                APIResult result = await APIAdapter.GetFromF1API($"{year}/{Round}/results");

                if (result.Equals(APIResult.Fail))
                {
                    throw new ScenarioLoadException($"Failed to fetch event data for {this}");
                }
                JArray driversJson = result.Data["MRData"]["RaceTable"]["Races"][0]["Results"].ToObject<JArray>();

                Model = new EventModel(Track, EventDate.Year, driversJson);

                // Assign current number of laps to match model
                NumLaps = Model.NumLaps;

                IsLoaded = true;
                Logger.Instance.Info($"Loaded race data for the {EventName}");
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
        public bool Equals(Scenario other)
        {
            return Guid.Equals(Id, other.Id);
        }

        public override string ToString()
        {
            return EventName;
        }

    }
}
