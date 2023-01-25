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
using WhatIfF1.Modelling.Tracks.Interfaces;
using WhatIfF1.Scenarios.Events;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Scenarios.Interfaces;
using WhatIfF1.UI.Controller;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Scenarios
{
    public class Scenario : LoadableBindableBase, IScenario
    {
        /// <summary>
        /// Used for equality checks
        /// </summary>
        public Guid Id { get; }
        public string EventName { get; }
        public ITrack Track { get; }
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

        private ICommand _loadRaceCommand;

        public ICommand LoadRaceCommand
        {
            get
            {
                return _loadRaceCommand ?? (_loadRaceCommand = new CommandHandler(
                        () => LoadRace(),
                        () => !IsLoading && !IsLoaded));
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
                return _removeScenarioCommand ?? (_removeScenarioCommand = new CommandHandler(() => ScenarioStore.Instance.RemoveScenario(this), () => true));
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
            get => _cloneScenarioCommand ?? (_cloneScenarioCommand = new CommandHandler(() => ScenarioStore.Instance.CloneScenario(this), () => true));
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

        private IEventController _eventController;

        public IEventController EventController
        {
            get => _eventController;
            set
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

        private async void LoadRace()
        {
            Logger.Instance.Info($"Loading race data for the {EventName}");
            IsLoading = true;

            try
            {
                var driverApiTask = APIAdapter.GetFromErgastAPI($"{EventDate.Year}/{Round}/results.json");
                var lapTimesApiTask = APIAdapter.GetFromErgastAPI($"{EventDate.Year}/{Round}/laps.json?limit=10000");
                var telemetryApiTask = APIAdapter.GetTelemetryJsonFromLiveTimingAPI(EventName, EventDate);

                var driverWorker = new APIEventCacheWorker(driverApiTask, "Drivers", "Drivers", $"{EventDate.Year}-{Round:D2}.json");
                var lapTimesWorker = new APIEventCacheWorker(lapTimesApiTask, "LapTimes", "Lap Times", $"{EventDate.Year}-{Round:D2}.json");
                var telemetryWorker = new APIEventCacheWorker(telemetryApiTask, "Telemetry", "Telemetry", $"{EventName}-{EventDate:dd-MM-yy}.json");

                var driverTask = driverWorker.GetDataTask();
                var lapTimesTask = lapTimesWorker.GetDataTask();
                var telemetryTask = telemetryWorker.GetDataTask();

                await Task.WhenAll(driverTask, lapTimesTask, telemetryApiTask);

                if (driverTask.IsFaulted || driverTask.Result.Equals(JsonFetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch driver data for {this}");
                }

                if (lapTimesTask.IsFaulted || lapTimesTask.Result.Equals(JsonFetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch lap time data for {this}");
                }

                if (telemetryTask.IsFaulted || telemetryTask.Result.Equals(JsonFetchResult.Fail))
                {
                    throw new ScenarioException($"Failed to fetch telemetry data for {this}");
                }

                JArray driverRaceTable = (JArray)driverTask.Result.Data["MRData"]["RaceTable"]["Races"];

                JArray driversJson = (JArray)driverRaceTable[0]["Results"];

                // Sometimes happens if the race has not yet occured (e.g race is in the future)
                if (driverRaceTable.Count == 0)
                {
                    throw new ScenarioException("No race data was found for the selected race. Has this race occured yet?");
                }

                JArray lapTimesJson = (JArray)lapTimesTask.Result.Data["MRData"]["RaceTable"]["Races"][0]["Laps"];
                JObject telemetryJson = (JObject)telemetryTask.Result.Data;

                string modelName = $"{EventDate.Year} - {EventName}";

                int numLaps = driversJson.Max((driver) => driver["laps"].ToObject<int>());

                // Create a new event model from the raw json
                EventModel model = await Task.Run(() => EventModel.GetModel(modelName, Track.TrackLength, driversJson, lapTimesJson, telemetryJson));

                // Create a new EventController using the event model
                EventController = new EventController(Track, model);

                IsLoaded = true;
                ScenarioLoaded?.Invoke(this, new ScenarioLoadedEventArgs(this));

                Logger.Instance.Info($"Loaded race data for the {EventName}");
            }
            catch (ScenarioException e)
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

        public bool Equals(IScenario other)
        {
            return Equals(Id, other.Id);
        }

        public override string ToString()
        {
            return EventName;
        }

        public event EventHandler<ScenarioLoadedEventArgs> ScenarioLoaded;
    }
}
