using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatIfF1.Adapters;
using WhatIfF1.Scenarios.Events;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Scenarios.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.Scenarios
{
    public sealed class ScenarioStore : NotifyPropertyChangedWrapper
    {
        private const int _year = 2022;

        #region StaticInitialization

        public static ScenarioStore Instance { get; private set; }

        public static async Task<ScenarioStore> InitialiseASync()
        {
            if (Instance != null)
            {
                throw new ScenarioException($"{nameof(ScenarioStore)} singleton has already been initialised");
            }

            var apiFetchTask = APIAdapter.GetFromErgastAPI($"{_year}.json");
            var scenarioWorker = new APIEventCacheWorker(apiFetchTask, "Events", "Events", $"{_year}.json");

            JsonFetchResult result = await scenarioWorker.GetDataTask();

            if (!result.Success)
            {
                throw new ScenarioException($"Failed to load scenarios from {_year} as the data load call failed");
            }

            JToken rawJson = result.Data;

            // Access inner race table data from the response
            JArray races = (JArray)rawJson["MRData"]["RaceTable"]["Races"];

            ICollection<IScenario> scenarios = new List<IScenario>(races.Count);

            foreach (JObject raceJson in races.Cast<JObject>())
            {
                scenarios.Add(new Scenario(raceJson));
            }

            Instance = new ScenarioStore(scenarios);

            return Instance;
        }

        #endregion StaticInitialization

        public ObservableRangeCollection<IScenario> Scenarios { get; }

        private IScenario _activeScenario;

        public IScenario ActiveScenario
        {
            get => _activeScenario;
            set
            {
                if (_activeScenario?.Equals(value) == true)
                {
                    return;
                }
                _activeScenario = value;
                OnPropertyChanged();
            }
        }

        private int _selectedAccordianIndex;

        public int SelectedAccordianIndex
        {
            get => _selectedAccordianIndex;
            set
            {
                if (_selectedAccordianIndex == value)
                {
                    return;
                }
                _selectedAccordianIndex = value;
                OnPropertyChanged();
            }
        }

        private ScenarioStore(IEnumerable<IScenario> scenarios)
        {
            Scenarios = new ObservableRangeCollection<IScenario>(scenarios);

            // Add event listeners for each scenario
            foreach (var scenario in Scenarios)
            {
                scenario.ScenarioLoaded += Scenario_ScenarioLoaded;
            }
            // Auto select the first scenario
            if (Scenarios.Count > 0)
            {
                ActiveScenario = Scenarios[0];
            }
        }

        private void Scenario_ScenarioLoaded(object sender, ScenarioLoadedEventArgs e)
        {
            ActiveScenario = Scenarios.Single(scenario => scenario.Equals((IScenario)sender));
            SelectedAccordianIndex = 1;
        }

        public void RemoveScenario(IScenario scenario)
        {
            Scenarios.Remove(scenario);
        }

        public void CloneScenario(IScenario original)
        {
            Scenarios.Add((IScenario)original.Clone());
        }
    }
}
