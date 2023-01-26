using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Graphing.SeriesData.Interfaces;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Graphing.Interfaces
{
    public interface IXYGraph
    {
        ObservableRangeCollection<IXYDataPoint<double>> Data { get; }

        IDriver TargetDriver { get; set; }

        string XTitle { get; set; }
        string YTitle { get; set; }

        double MaxY { get; }
        double MinY { get; }

        void Clear();

        void UpdateGraph();
    }
}
