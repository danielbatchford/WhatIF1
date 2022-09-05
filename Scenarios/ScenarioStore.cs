using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Adapters;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Util;

namespace WhatIfF1.Scenarios
{
    public class ScenarioStore : PropertyChangedWrapper
    {
        private static readonly int _year = 2022;

        #region LazyInitialization

        private static readonly Task<ScenarioStore> _getInstanceTask = GetScenarioStoreASync(_year);
        public static Task<ScenarioStore> Instance => _getInstanceTask;

        #endregion LazyInitialization

        public ObservableCollection<Scenario> Scenarios { get; }

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

        private static async Task<ScenarioStore> GetScenarioStoreASync(int year)
        {

            APIResult result = await APIAdapter.GetFromF1API(_year.ToString());

            if (!result.Success)
            {
                throw new ScenarioLoadException($"Failed to load scenarios from {year} as the API call failed");
            }

            JObject rawJson = result.Data;

            // Access inner race table data from the response
            JArray races = (JArray)rawJson["MRData"]["RaceTable"]["Races"];

            ICollection<Scenario> scenarios = new List<Scenario>(races.Count);

            foreach (JObject raceJson in races.Cast<JObject>())
            {
                scenarios.Add(new Scenario(raceJson));
            }

            return new ScenarioStore(scenarios);
        }

        private ScenarioStore(IEnumerable<Scenario> scenarios)
        {
            Scenarios = new ObservableCollection<Scenario>(scenarios);
        }
    }
}
