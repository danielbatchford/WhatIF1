using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Graphing.SeriesData.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing.Interfaces
{
    public interface IXYGraph
    {
        IList<IXYDataPoint<double>> Data { get; }

        IDriver TargetDriver { get; set; }

        string XTitle { get; set; }
        string YTitle { get; set; }

        void Clear();

        void UpdateGraph();
    }
}
