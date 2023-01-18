using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.UI.Controller;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Scenarios
{
    public class Scenario : NotifyPropertyChangedWrapper, ICloneable, IEquatable<Scenario>
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
            protected set
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
                // Fetches driver data for the event
                Task < FetchResult > driverTask = APIAdapter.GetFromErgastAPI($"{EventDate.Year}/{Round}/results.json");

                // Fetches lap time data for the event
                Task<FetchResult> lapTimesTask = APIAdapter.GetFromErgastAPI($"{EventDate.Year}/{Round}/laps.json?limit=10000");

                // Fetches telemetry data for the event, from the cache if possible
                Task<FetchResult> telemetryTask;

                bool telemetryCacheFileExists = FileAdapter.Instance.TelemetryCacheFileExists(EventName, EventDate);
                if (telemetryCacheFileExists)
                {
                    Logger.Instance.Info("Fetching telemetry data from cache");
                    telemetryTask = FileAdapter.Instance.LoadTelemetryCacheFileAsync(EventName, EventDate); 
                }
                else
                {
                    Logger.Instance.Info("Fetching telemetry data from live timing API");
                    telemetryTask = APIAdapter.GetTelemetryJsonFromLiveTimingAPI(EventName, EventDate);
                }

                await Task.WhenAll(driverTask, lapTimesTask, telemetryTask);

                if (driverTask.IsFaulted || driverTask.Result.Equals(FetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch driver data for {this}");
                }

                if (lapTimesTask.IsFaulted || lapTimesTask.Result.Equals(FetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch lap time data for {this}");
                }

                if (telemetryTask.IsFaulted || telemetryTask.Result.Equals(FetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch telemetry data for {this}");
                }

                JArray driverRaceTable = driverTask.Result.Data["MRData"]["RaceTable"]["Races"].ToObject<JArray>();

                JArray driversJson = driverRaceTable[0]["Results"].ToObject<JArray>();

                // Sometimes happens if the race has not yet occured (e.g race is in the future)
                if (driverRaceTable.Count == 0)
                {
                    throw new ScenarioException("No race data was found for the selected race. Has this race occured yet?");
                }

                JArray lapTimesJson = lapTimesTask.Result.Data["MRData"]["RaceTable"]["Races"][0]["Laps"].ToObject<JArray>();
                JArray telemetryJson = (JArray)telemetryTask.Result.Data;

                // Cache this telemetry data if it is not already cached
                if (!telemetryCacheFileExists)
                {
                    Logger.Instance.Info("Writing telemetry data to cache");
                    _ = Task.Run(() => FileAdapter.Instance.WriteTelemetryCacheFile(EventName, EventDate, telemetryJson));
                }

                string modelName = $"{EventDate.Year} - {EventName}";

                int numLaps = driversJson.Max((driver) => driver["laps"].ToObject<int>());

                // Create a new event model from the raw json
                EventModel model = new EventModel(modelName, Track.TrackLength, driversJson, lapTimesJson, telemetryJson);

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
