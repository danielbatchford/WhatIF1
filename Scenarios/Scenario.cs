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
using WhatIfF1.UI.Controller;
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

        public ScenarioType ScenarioType { get; }

        public int Round { get; }

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

        private bool _isModelLoading;

        public bool IsModelLoading
        {
            get => _isModelLoading;
            private set
            {
                _isModelLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isModelLoaded;

        public bool IsModelLoaded
        {
            get => _isModelLoaded;
            private set
            {
                _isModelLoaded = value;
                OnPropertyChanged();
            }
        }

        private ICommand _loadRaceCommand;

        public ICommand LoadRaceCommand
        {
            get
            {
                if (_loadRaceCommand is null)
                {
                    _loadRaceCommand = new CommandHandler(
                        () => LoadRaceFromAPI(),
                        () => !IsModelLoading && !IsModelLoaded);
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
                if (_cloneScenarioCommand is null)
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

        private EventController _eventController;

        public EventController EventController
        {
            get => _eventController;
            private set
            {
                _eventController = value;
                OnPropertyChanged();
            }
        }

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
            using (System.Drawing.Image flag = System.Drawing.Image.FromFile(Track.FlagFilePath))
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

            // Set scenario type to a race type
            ScenarioType = ScenarioType.RACE;
        }

        private async void LoadRaceFromAPI()
        {
            Logger.Instance.Info($"Loading race data for the {EventName}");
            IsModelLoading = true;

            try
            {
                int year = EventDate.Year;

                // Fetch driver data for the event
                APIResult driverResult = await APIAdapter.GetFromF1API($"{year}/{Round}/results");

                if (driverResult.Equals(APIResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch driver data for {this}");
                }

                JArray driverRaceTable = driverResult.Data["MRData"]["RaceTable"]["Races"].ToObject<JArray>();

                // Sometimes happens if the race has not yet occured (e.g race is in the future)
                if (driverRaceTable.Count == 0)
                {
                    throw new ScenarioException("No race data was found for the selected race. Has this race occured yet?");
                }

                JArray driversJson = driverRaceTable[0]["Results"].ToObject<JArray>();

                // Fetch lap time data for the event
                APIResult lapTimesResult = await APIAdapter.GetFromF1API($"{year}/{Round}/laps.json");

                if (lapTimesResult.Equals(APIResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch lap time data for {this}");
                }

                JArray lapTimesJson = lapTimesResult.Data["MRData"]["RaceTable"]["Races"][0]["Laps"].ToObject<JArray>();

                string modelName = $"{year} - {EventName}";

                // Create a new event model from the raw json
                EventModel model = new EventModel(modelName, Track.TrackLength, year, driversJson, lapTimesJson);

                // Create a new EventController using the event model
                EventController = new EventController(Track, model);

                IsModelLoaded = true;
                Logger.Instance.Info($"Loaded race data for the {EventName}");
            }
            catch (ScenarioException e)
            {
                Logger.Instance.Exception(e);
            }
            finally
            {
                IsModelLoading = false;
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
