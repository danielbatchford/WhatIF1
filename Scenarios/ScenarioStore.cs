using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatIfF1.Adapters;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.Scenarios
{
    public sealed class ScenarioStore : NotifyPropertyChangedWrapper
    {
        private static readonly int _year = 2022;

        #region StaticInitialization

        public static ScenarioStore Instance { get; private set; }

        public static async Task<ScenarioStore> InitialiseASync()
        {
            if (Instance != null)
            {
                throw new ScenarioException($"{nameof(ScenarioStore)} singleton has already been initialised");
            }

            FetchResult result = await APIAdapter.GetFromErgastAPI($"{_year.ToString()}.json");

            if (!result.Success)
            {
                throw new ScenarioException($"Failed to load scenarios from {_year} as the API call failed");
            }

            JToken rawJson = result.Data;

            // Access inner race table data from the response
            JArray races = (JArray)rawJson["MRData"]["RaceTable"]["Races"];

            ICollection<Scenario> scenarios = new List<Scenario>(races.Count);

            foreach (JObject raceJson in races.Cast<JObject>())
            {
                scenarios.Add(new Scenario(raceJson));
            }

            Instance = new ScenarioStore(scenarios);

            return Instance;
        }

        #endregion StaticInitialization

        public ObservableRangeCollection<Scenario> Scenarios { get; }

        private Scenario _activeScenario;

        public Scenario ActiveScenario
        {
            get => _activeScenario;
            set
            {
                _activeScenario = value;
                OnPropertyChanged();
            }
        }

        private ScenarioStore(IEnumerable<Scenario> scenarios)
        {
            Scenarios = new ObservableRangeCollection<Scenario>(scenarios);

            // Auto select the first scenario
            if (Scenarios.Count > 0)
            {
                ActiveScenario = Scenarios[0];
            }
        }

        public void RemoveScenario(Scenario scenario)
        {
            Scenarios.Remove(scenario);
        }

        public void CloneScenario(Scenario original)
        {
            Scenarios.Add((Scenario)original.Clone());
        }
    }
}
