using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing.Interfaces
{
    public interface IGraphProvider
    {
        IEnumerable<GraphType> GraphTypes { get; }

        void UpdateGraph();

        void UpdateCurrentDriver(IDriver driver);

        void RemoveCurrentDriver();
    }
}
