﻿using System;
using System.Windows.Media;
using WhatIfF1.Modelling.Tracks.Interfaces;
using WhatIfF1.Scenarios.Events;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.Scenarios.Interfaces
{
    public interface IScenario : ICloneable, IEquatable<IScenario>
    {
        Guid Id { get; }
        string EventName { get; }
        ITrack Track { get; }
        DateTime EventDate { get; }
        string WikipediaLink { get; }

        ScenarioType ScenarioType { get; }

        int Round { get; }

        Color PrimaryColor { get; set; }

        Color SecondaryColor { get; set; }
        bool IsLoading { get; set; }

        bool IsLoaded { get; set; }

        IEventController EventController { get; }

        event EventHandler<ScenarioLoadedEventArgs> ScenarioLoaded;
    }
}
