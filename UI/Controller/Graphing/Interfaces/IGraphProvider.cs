using System.Collections.Generic;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing.Interfaces
{
    public interface IGraphProvider : ITargetDriverDataProvider
    {
        IEnumerable<GraphType> GraphTypes { get; }
    }
}
