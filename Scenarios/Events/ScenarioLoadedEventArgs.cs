using System;
using WhatIfF1.Scenarios.Interfaces;

namespace WhatIfF1.Scenarios.Events
{
    public class ScenarioLoadedEventArgs : EventArgs
    {
        public IScenario Scenario { get; }

        public ScenarioLoadedEventArgs(IScenario scenario)
        {
            Scenario = scenario;
        }
    }
}
