using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class VelocityLapGraph : LapGraph
    {
        public VelocityLapGraph(IEventController parentController) : base(parentController, "Velocity")
        {
        }
    }
}
