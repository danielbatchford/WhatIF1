using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Util;

namespace WhatIfF1.Scenarios
{
    public class ScenarioStore : PropertyChangedWrapper
    {
        #region LazyInitialization

        public static ScenarioStore Instance => _lazy.Value;

        private readonly static Lazy<ScenarioStore> _lazy = new Lazy<ScenarioStore>(() => new ScenarioStore());



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


        private ScenarioStore()
        {
            Scenarios = new ObservableCollection<Scenario>
            {
                new Scenario("Hello", 2022)
            };
        }
    }
}
